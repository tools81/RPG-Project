using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Movement;
using System;
using Unity.VisualScripting;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {             
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;   

        Mover mover;
        Health target;
        Weapon currentWeapon = null;
        float timeSinceLastAttack = Mathf.Infinity;

        private void Start() 
        {
            mover = GetComponent<Mover>();

            if (currentWeapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
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
            if (timeSinceLastAttack > currentWeapon.GetTimeBetweenAttacks())
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

            var handTransform = currentWeapon.GetHandTransform(rightHandTransform, leftHandTransform);

            if (currentWeapon.AttackEffect != null)
            {
                Instantiate(currentWeapon.AttackEffect, handTransform.position, handTransform.rotation);
            }

            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject);
            }
            else
            {
                if (currentWeapon.ImpactEffect != null)
                {
                    Instantiate(currentWeapon.ImpactEffect, target.GetAimLocation(), target.transform.rotation);
                }
                target.TakeDamage(gameObject, currentWeapon.GetWeaponDamage());
            }
        }

        //Shoot is another animation event. We do the same thing so just call Hit
        private void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetWeaponRange();
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            var animator = GetComponent<Animator>();
            currentWeapon.Spawn(rightHandTransform, leftHandTransform, animator);
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

        public object CaptureState()
        {
            return currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            var weaponName = (string)state;
            var weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}