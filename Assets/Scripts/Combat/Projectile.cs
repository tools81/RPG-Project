using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {    
        [SerializeField] float speed = 20;
        [SerializeField] bool homing = false;
        [SerializeField] GameObject impactEffect = null;
        [SerializeField] float maxLifetime = 10f;
        //The following 2 fields allow us to destroy effects on the projectile separately. Must add at least one to array or projectile will live past collision.
        [SerializeField] GameObject[] destroyOnImpact = null;
        [SerializeField] float lifeAfterImpact = 2f;

        Health target = null;
        GameObject instigator = null;
        float damage = 0;

        private void Start() 
        {
            transform.LookAt(target.GetAimLocation());
        }

        private void Update() 
        {
            if (target == null) return;

            if (homing && !target.IsDead())
            {
                transform.LookAt(target.GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;           
            this.instigator = instigator;
            this.damage = damage;

            Destroy(gameObject, maxLifetime);
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;

            target.TakeDamage(instigator, damage);
            speed = 0;

            if (impactEffect != null)
            {
                Instantiate(impactEffect, target.GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnImpact)
            {
                Destroy(toDestroy);
            }
            
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
