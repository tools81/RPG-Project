using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Movement;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
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

            EquipWeapon(defaultWeapon);
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
            target.TakeDamage(currentWeapon.GetWeaponDamage());
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
    }
}