using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;

        public event Action onLevelUp;

        int currentLevel = 0;

        private void Start() 
        {
            currentLevel = CalculateLevel();
            Experience experience = GetComponent<Experience>();
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;                
            }
        }

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public int GetLevel()
        {
            if (currentLevel < 1)
            {
                currentLevel = CalculateLevel();
            }
            return currentLevel;
        }

        public int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;

            var currentExperience = experience.GetExperiencePoints();
            var penultimateLevel = progression.GetLevels(Stat.ExperienceToLevel, characterClass);

            for (int level = 1; level <= penultimateLevel; level++)
            {
                var experienceToLevel = progression.GetStat(Stat.ExperienceToLevel, characterClass, level);
                if (experienceToLevel > currentExperience)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }

        private void UpdateLevel() 
        {
            int newLevel = CalculateLevel();    
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }        
    }
}
