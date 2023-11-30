using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFileName = "save";

        JsonSavingSystem savingSystem;

        private void Awake()
        {
            savingSystem = GetComponent<JsonSavingSystem>();
            //yield return savingSystem.LoadLastScene(defaultSaveFileName);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Save()
        {
            savingSystem.Save(defaultSaveFileName);
        }

        public void Load()
        {
            savingSystem.Load(defaultSaveFileName);
        }

        public void Delete()
        {
            GetComponent<JsonSavingSystem>().Delete(defaultSaveFileName);
        }
    }
}