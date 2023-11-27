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
        float healthPoints = -1f;


        bool isDead = false;

        private void Start() 
        {
            if (healthPoints < 0)
            {
                healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);          
            }
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

        public float GetPercentage()
        {
            return Mathf.Round(100 * (healthPoints / GetComponent<BaseStats>().GetStat(Stat.Health)));
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

        private void AwardExperience(GameObject instigator)
        {
            var experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }
    }
}
