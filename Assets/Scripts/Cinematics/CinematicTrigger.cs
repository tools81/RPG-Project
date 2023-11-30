using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, IJsonSaveable
    {
        bool hasTriggered = false;        

        private void OnTriggerEnter(Collider other) 
        {
            if (!hasTriggered && other.gameObject.tag == "Player")
            {
                hasTriggered = true;
                GetComponent<PlayableDirector>().Play();
            }
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(hasTriggered);
        }

        public void RestoreFromJToken(JToken state)
        {
            hasTriggered = state.ToObject<bool>();
        }
    }
}
