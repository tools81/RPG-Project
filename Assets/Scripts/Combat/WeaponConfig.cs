using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject 
    {
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float weaponPercentageBonus = 0f;
        [SerializeField] float timeBetweenAttacks = 1.2f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;
        [field: SerializeField] public GameObject AttackEffect { get; set; }
        [field: SerializeField] public GameObject ImpactEffect { get; set; }
        [SerializeField] AnimatorOverrideController weaponAnimationOverride = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            Weapon weapon = null;

            if (equippedPrefab != null)
            {
                Transform handTransform = GetHandTransform(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.name = weaponName;
            }

            if (weaponAnimationOverride != null)
            {
                animator.runtimeAnimatorController = weaponAnimationOverride;
            }
            else
            {
                var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                if (overrideController != null)
                {
                    animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
                }
            }

            return weapon;
        }       

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            var projectileInstance = Instantiate(projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return weaponPercentageBonus;
        }

        public float GetTimeBetweenAttacks()
        {
            return timeBetweenAttacks;
        }

        public GameObject GetAttackEffect()
        {
            return AttackEffect;
        }

        public GameObject GetImpactEffect()
        {
            return ImpactEffect;
        }

        public Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            return isRightHanded ? rightHand : leftHand;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName) != null ? rightHand.Find(weaponName) : leftHand.Find(weaponName);

            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYED";
            Destroy(oldWeapon.gameObject);
        }
    }
}
