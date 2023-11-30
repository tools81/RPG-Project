using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] float experiencePoints = 0;  

        public event Action onExperienceGained;    

        public void GainExperience(float experienceReward)
        {
            experiencePoints += experienceReward;
            onExperienceGained();
        }

        public float GetExperiencePoints()
        {
            return experiencePoints;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(experiencePoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            experiencePoints = state.ToObject<float>();
        }
    }
}
