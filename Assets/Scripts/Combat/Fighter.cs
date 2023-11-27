using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Movement;
using System;
using Unity.VisualScripting;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {             
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;   

        Mover mover;
        Health target;
        LazyValue<Weapon> currentWeapon = null;
        float timeSinceLastAttack = Mathf.Infinity;

        private void Awake() 
        {
            mover = GetComponent<Mover>();
            currentWeapon = new LazyValue<Weapon>(GetCurrentWeapon);
        }

        private void Start() 
        {
            currentWeapon.ForceInit();
        }        

        private void Update() 
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null || target.IsDead()) return;

            if (!GetIsInRange())
            {
                mover.MoveTo(target.transform.position);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > currentWeapon.value.GetTimeBetweenAttacks())
            {
                ResetAndTriggerAttackEvent("stopAttack", "attack");
                timeSinceLastAttack = 0f;
            }
        }

        private void ResetAndTriggerAttackEvent(string trigger1, string trigger2)
        {
            GetComponent<Animator>().ResetTrigger(trigger1);
            GetComponent<Animator>().SetTrigger(trigger2);
        }

        //Animation Event with Attack Trigger
        private void Hit()
        {
            if (target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            var handTransform = currentWeapon.value.GetHandTransform(rightHandTransform, leftHandTransform);

            if (currentWeapon.value.AttackEffect != null)
            {
                Instantiate(currentWeapon.value.AttackEffect, handTransform.position, handTransform.rotation);
            }

            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                if (currentWeapon.value.ImpactEffect != null)
                {
                    Instantiate(currentWeapon.value.ImpactEffect, target.GetAimLocation(), target.transform.rotation);
                }                
                target.TakeDamage(gameObject, damage);
            }
        }

        //Shoot is another animation event. We do the same thing so just call Hit
        private void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.value.GetWeaponRange();
        }

        private Weapon GetCurrentWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            var animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {            
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            ResetAndTriggerAttackEvent("attack", "stopAttack");
            target = null;
            mover.Cancel();
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }

        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            var weaponName = (string)state;
            var weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }       
    }
}