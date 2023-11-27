using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regeneratePercentage = 70f;
        LazyValue<float> healthPoints;


        bool isDead = false;

        private void Awake() 
        {
            healthPoints = new LazyValue<float>(GetMaxHealthPoints);    
        }

        private void Start() 
        {            
            healthPoints.ForceInit();
        } 

        private void OnEnable() 
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        } 

        private void OnDisable() 
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }      

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (healthPoints.value <= 0) return;

            print(gameObject.name + " took damage: " + damage);

            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);

            if (healthPoints.value < 1)
            {
                Die();
                AwardExperience(instigator);
            }
        } 

        public float GetCurrentHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }    

        public float GetPercentage()
        {
            return Mathf.Round(100 * (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health)));
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
            healthPoints.value = (float)state;
            if (healthPoints.value < 1) Die();
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

        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regeneratePercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }
    }
}
