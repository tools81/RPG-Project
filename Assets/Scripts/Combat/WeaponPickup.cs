using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon weapon = null;
        [SerializeField] float respawnTime = 5f;

        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.tag == "Player")
            {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                StartCoroutine(HideForSeconds(respawnTime));
            }
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            PickupSpawningVisibility(false);
            yield return new WaitForSeconds(respawnTime);
            PickupSpawningVisibility(true);
        }       

        private void PickupSpawningVisibility(bool visible)
        {
            gameObject.GetComponent<Collider>().enabled = visible;
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(visible);
            }
        }
    }
}
