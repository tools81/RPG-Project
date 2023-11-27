using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Health health;

        private void Awake() 
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();    
        }

        private void Update() 
        {
            GetComponent<TextMeshProUGUI>().text = $"{health.GetCurrentHealthPoints()}/{health.GetMaxHealthPoints()} - {health.GetPercentage()}%";
        }
    }
}
