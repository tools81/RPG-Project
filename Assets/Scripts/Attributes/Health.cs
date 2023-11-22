using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float healthPoints = 100f;


        bool isDead = false;

        private void Start() 
        {
            healthPoints = GetComponent<BaseStats>().GetHealth();
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (healthPoints <= 0) return;

            healthPoints = Mathf.Max(healthPoints - damage, 0);

            if (healthPoints < 1)
            {
                Die();
                AwardExperience(instigator);
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            var experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetExperienceReward());
        }

        public float GetPercentage()
        {
            return Mathf.Round(100 * (healthPoints / GetComponent<BaseStats>().GetHealth()));
        }

        public Vector3 GetAimLocation()
        {
            var capsule = transform.GetComponent<CapsuleCollider>();
            if (capsule == null) return transform.position;

            return transform.position + Vector3.up * capsule.height / 2;
        }        

        public object CaptureState()
        {
            return healthPoints;
        }

        public void RestoreState(object state)
        {
            healthPoints = (float)state;
            if (healthPoints < 1) Die();
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
