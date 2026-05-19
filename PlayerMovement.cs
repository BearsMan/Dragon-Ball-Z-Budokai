using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Budokai
{
    public enum CombatState
    {
        Neutral,
        Attacking,
        Recovering,
        Stunned,
        Launched,
        Airborne,
        Downed,
        Guarding,
        ChargingKi
    }

    public enum HitType
    {
        Light,
        Heavy,
        Launcher,
        Knockback,
        Super
    }

    [System.Serializable]
    public class AttackData
    {
        public string name;
        public string animatorTrigger;
        public HitType hitType;
        public float damage = 10f;
        public float chipDamage = 2f;
        public float hitStun = 0.4f;
        public float blockStun = 0.2f;
        public float knockbackDistance = 2f;
        public float launchForce = 6f;
        public float kiGainOnHit = 5f;
        public float kiCost = 0f;
        public bool canChainIntoNext = true;

        // Combo chaining
        public AttackData nextLight;   // next P
        public AttackData nextHeavy;   // next K
        public AttackData nextSpecial; // next special
    }

    [RequireComponent(typeof(Animator))]
    public class BudokaiPlayerController : MonoBehaviour
    {
        [Header("References")]
        public Transform opponent;
        public Animator anim;

        [Header("Movement Settings")]
        public float forwardStepSpeed = 4f;
        public float backwardStepSpeed = 3f;
        public float sidestepSpeed = 6f;
        public float dashSpeed = 12f;
        public float autoAlignSpeed = 10f;

        [Header("Combat Settings")]
        public float maxHealth = 100f;
        public float maxKi = 100f;
        public float kiChargeRate = 20f;
        public float guardKiDrainRate = 10f;
        public float teleportKiCost = 25f;
        public float teleportDistance = 3f;
        public LayerMask wallMask;

        [Header("Character Combo Trees")]
        public AttackData lightStarter;     // P
        public AttackData heavyStarter;     // K
        public AttackData launcherAttack;   // Up+K
        public AttackData superAttack;      // Special

        // Example: Character-specific combo roots
        [Header("Character Combo Roots")]
        public AttackData gokuRoot;
        public AttackData vegetaRoot;
        public AttackData piccoloRoot;
        public AttackData gohanRoot;
        public AttackData krillinRoot;
        public AttackData yamchaRoot;
        public AttackData tienRoot;
        public AttackData nappaRoot;
        public AttackData raditzRoot;
        public AttackData zarbonRoot;
        public AttackData friezaRoot;
        public AttackData cellRoot;
        public AttackData android18Root;
        public AttackData android16Root;
        public AttackData trunksRoot;
        public AttackData greatSaiyamanRoot;
        public AttackData herculeRoot;

        private CombatState state = CombatState.Neutral;
        private float currentHealth;
        private float currentKi;

        private bool isDashing;
        private bool isFacingRight;

        private AttackData currentAttack;
        private AttackData bufferedAttack;

        private bool canInputNext;

        private void Awake()
        {
            if (!anim) anim = GetComponent<Animator>();
            currentHealth = maxHealth;
            currentKi = 0f;
        }

        private void Update()
        {
            if (!opponent) return;

            HandleAutoAlignment();

            switch (state)
            {
                case CombatState.Neutral:
                case CombatState.Guarding:
                case CombatState.ChargingKi:
                    HandleMovementInput();
                    HandleNeutralCombatInput();
                    break;

                case CombatState.Attacking:
                    HandleComboBuffer();
                    break;
            }

            HandleGlobalInputs();
        }

        #region Movement

        private void HandleAutoAlignment()
        {
            Vector3 dir = opponent.position - transform.position;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, autoAlignSpeed * Time.deltaTime);
            }

            isFacingRight = Vector3.Dot(transform.right, dir.normalized) > 0;
        }

        private void HandleMovementInput()
        {
            if (state == CombatState.ChargingKi) return;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            anim.SetFloat("Horizontal", h);
            anim.SetFloat("Vertical", v);

            if (v > 0) StepForward();
            else if (v < 0) StepBackward();

            if (Mathf.Abs(h) > 0.1f) Sidestep(h);

            if (Input.GetButtonDown("Fire3")) Dash();
        }

        private void StepForward() => transform.position += transform.forward * forwardStepSpeed * Time.deltaTime;
        private void StepBackward() => transform.position -= transform.forward * backwardStepSpeed * Time.deltaTime;
        private void Sidestep(float dir) => transform.position += transform.right * dir * sidestepSpeed * Time.deltaTime;

        private void Dash()
        {
            if (isDashing || state != CombatState.Neutral) return;

            isDashing = true;
            anim.SetTrigger("Dash");
            StartCoroutine(DashRoutine());
        }

        private IEnumerator DashRoutine()
        {
            float t = 0;
            while (t < 0.25f)
            {
                transform.position += transform.forward * dashSpeed * Time.deltaTime;
                t += Time.deltaTime;
                yield return null;
            }
            isDashing = false;
        }

        #endregion

        #region Input Handling

        private void HandleNeutralCombatInput()
        {
            bool guardHeld = Input.GetButton("Fire4");

            if (guardHeld && state != CombatState.ChargingKi) StartGuard();
            else if (!guardHeld && state == CombatState.Guarding) EndGuard();

            if (Input.GetButton("Fire4") && Input.GetButton("Jump")) StartKiCharge();
            else if (state == CombatState.ChargingKi && !Input.GetButton("Fire4")) EndKiCharge();

            if (Input.GetButtonDown("Fire1")) TryStartAttack(lightStarter);
            if (Input.GetButtonDown("Fire2")) TryStartAttack(heavyStarter);

            if (Input.GetButtonDown("Fire2") && Input.GetAxisRaw("Vertical") > 0.1f)
                TryStartAttack(launcherAttack);

            if (Input.GetButtonDown("Submit")) TryStartAttack(superAttack);
        }

        private void HandleComboBuffer()
        {
            if (!canInputNext || currentAttack == null) return;

            if (Input.GetButtonDown("Fire1") && currentAttack.nextLight != null)
                bufferedAttack = currentAttack.nextLight;

            if (Input.GetButtonDown("Fire2") && currentAttack.nextHeavy != null)
                bufferedAttack = currentAttack.nextHeavy;

            if (Input.GetButtonDown("Submit") && currentAttack.nextSpecial != null)
                bufferedAttack = currentAttack.nextSpecial;
        }

        private void HandleGlobalInputs()
        {
            if (Input.GetButtonDown("Fire4") && Input.GetButtonDown("Submit"))
                TryTeleportBehind();
        }

        #endregion

        #region Guard & Ki

        private void StartGuard()
        {
            if (state != CombatState.Neutral) return;
            state = CombatState.Guarding;
            anim.SetBool("Guard", true);
        }

        private void EndGuard()
        {
            if (state != CombatState.Guarding) return;
            state = CombatState.Neutral;
            anim.SetBool("Guard", false);
        }

        private void StartKiCharge()
        {
            if (state == CombatState.ChargingKi) return;
            state = CombatState.ChargingKi;
            anim.SetBool("ChargingKi", true);
            StartCoroutine(KiChargeRoutine());
        }

        private void EndKiCharge()
        {
            if (state != CombatState.ChargingKi) return;
            state = CombatState.Neutral;
            anim.SetBool("ChargingKi", false);
        }

        private IEnumerator KiChargeRoutine()
        {
            while (state == CombatState.ChargingKi)
            {
                currentKi = Mathf.Min(maxKi, currentKi + kiChargeRate * Time.deltaTime);
                yield return null;
            }
        }

        private void TryTeleportBehind()
        {
            if (currentKi < teleportKiCost) return;

            currentKi -= teleportKiCost;

            Vector3 dir = (opponent.position - transform.position).normalized;
            Vector3 target = opponent.position - dir * teleportDistance;
            target.y = transform.position.y;

            transform.position = target;
            anim.SetTrigger("Teleport");
        }

        #endregion

        #region Attacks

        private void TryStartAttack(AttackData attack)
        {
            if (attack == null) return;
            if (state != CombatState.Neutral && state != CombatState.Guarding && state != CombatState.ChargingKi) return;
            if (currentKi < attack.kiCost) return;

            currentKi -= attack.kiCost;

            currentAttack = attack;
            bufferedAttack = null;
            canInputNext = false;

            state = CombatState.Attacking;
            anim.SetTrigger(attack.animatorTrigger);
        }

        public void EnableComboWindow() => canInputNext = true;
        public void DisableComboWindow() => canInputNext = false;

        public void OnAttackHit()
        {
            if (!opponent) return;

            float dist = Vector3.Distance(transform.position, opponent.position);
            if (dist > 2.5f) return;

            BudokaiPlayerController other = opponent.GetComponent<BudokaiPlayerController>();
            if (!other) return;

            other.ReceiveHit(currentAttack, this);
            currentKi = Mathf.Clamp(currentKi + currentAttack.kiGainOnHit, 0, maxKi);
        }

        public void EndAttack()
        {
            if (bufferedAttack != null)
            {
                TryStartAttack(bufferedAttack);
                return;
            }

            currentAttack = null;
            bufferedAttack = null;

            state = CombatState.Recovering;
            StartCoroutine(RecoveryRoutine(0.1f));
        }

        private IEnumerator RecoveryRoutine(float t)
        {
            yield return new WaitForSeconds(t);
            if (state == CombatState.Recovering)
                state = CombatState.Neutral;
        }

        #endregion

        #region Hit Reactions

        public void ReceiveHit(AttackData attack, BudokaiPlayerController attacker)
        {
            if (state == CombatState.Guarding)
            {
                TakeGuardHit(attack);
                return;
            }

            TakeFullHit(attack, attacker);
        }

        private void TakeGuardHit(AttackData attack)
        {
            currentHealth -= attack.chipDamage;
            anim.SetTrigger("GuardHit");
            StartCoroutine(HitStunRoutine(attack.blockStun));

            if (currentKi <= 0)
            {
                anim.SetTrigger("GuardBreak");
                state = CombatState.Stunned;
                StartCoroutine(HitStunRoutine(0.8f));
            }
        }

        private void TakeFullHit(AttackData attack, BudokaiPlayerController attacker)
        {
            currentHealth -= attack.damage;

            switch (attack.hitType)
            {
                case HitType.Light:
                    anim.SetTrigger("HitLight");
                    state = CombatState.Stunned;
                    StartCoroutine(HitStunRoutine(attack.hitStun));
                    break;

                case HitType.Heavy:
                    anim.SetTrigger("HitHeavy");
                    state = CombatState.Stunned;
                    StartCoroutine(HitStunRoutine(attack.hitStun));
                    ApplyKnockback(attack, attacker);
                    break;

                case HitType.Launcher:
                    anim.SetTrigger("Launched");
                    state = CombatState.Launched;
                    StartCoroutine(LaunchRoutine(attack, attacker));
                    break;

                case HitType.Knockback:
                    anim.SetTrigger("Knockback");
                    state = CombatState.Stunned;
                    ApplyKnockback(attack, attacker, true);
                    StartCoroutine(HitStunRoutine(attack.hitStun));
                    break;

                case HitType.Super:
                    anim.SetTrigger("HitSuper");
                    state = CombatState.Stunned;
                    ApplyKnockback(attack, attacker, true);
                    StartCoroutine(HitStunRoutine(attack.hitStun + 0.3f));
                    break;
            }

            if (currentHealth <= 0) Die();
        }

        private IEnumerator HitStunRoutine(float t)
        {
            yield return new WaitForSeconds(t);
            if (state == CombatState.Stunned)
                state = CombatState.Neutral;
        }

        private void ApplyKnockback(AttackData attack, BudokaiPlayerController attacker, bool checkWall = false)
        {
            Vector3 dir = (transform.position - attacker.transform.position).normalized;
            dir.y = 0;

            Vector3 target = transform.position + dir * attack.knockbackDistance;

            if (checkWall)
            {
                if (Physics.Raycast(transform.position, dir, out RaycastHit hit, attack.knockbackDistance, wallMask))
                {
                    target = hit.point;
                    anim.SetTrigger("WallHit");
                }
            }

            StartCoroutine(KnockbackRoutine(target, 0.15f));
        }

        private IEnumerator KnockbackRoutine(Vector3 target, float t)
        {
            Vector3 start = transform.position;
            float time = 0;

            while (time < t)
            {
                transform.position = Vector3.Lerp(start, target, time / t);
                time += Time.deltaTime;
                yield return null;
            }

            transform.position = target;
        }

        private IEnumerator LaunchRoutine(AttackData attack, BudokaiPlayerController attacker)
        {
            Vector3 dir = (transform.position - attacker.transform.position).normalized;
            dir.y = 0;

            float t = 0;
            float duration = 0.4f;

            Vector3 start = transform.position;
            Vector3 apex = start + dir * 1.5f + Vector3.up * attack.launchForce * 0.2f;

            while (t < duration)
            {
                transform.position = Vector3.Lerp(start, apex, t / duration);
                t += Time.deltaTime;
                yield return null;
            }

            state = CombatState.Airborne;
            anim.SetBool("Airborne", true);
        }

        public void LandFromAir()
        {
            anim.SetBool("Airborne", false);
            state = CombatState.Neutral;
        }

        private void Die()
        {
            anim.SetTrigger("KO");
            state = CombatState.Downed;
        }

        #endregion
    }
}
