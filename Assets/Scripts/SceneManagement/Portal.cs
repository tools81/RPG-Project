using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D
        }

        [SerializeField] int sceneIndexToLoad = -1;
        [SerializeField] Transform portalSpawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeDuration = 0.5F;
        [SerializeField] float fadeWaitDuration = 0.5f;

        private void OnTriggerEnter(Collider other) 
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if (sceneIndexToLoad < 0)
            {
                Debug.LogError("No scene to load.");
                yield break;
            }            

            DontDestroyOnLoad(gameObject);

            var fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeDuration);

            var wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneIndexToLoad);

            wrapper.Load();
            
            Portal destinationPortal = GetDestinationPortal();
            UpdatePlayer(destinationPortal);
            
            wrapper.Save();

            yield return new WaitForSeconds(fadeWaitDuration);
            yield return fader.FadeIn(fadeDuration);

            Destroy(gameObject);            
        }

        private void UpdatePlayer(Portal destinationPortal)
        {
            if (destinationPortal == null)
            {
                Debug.LogError("No spawnpoint set for portal");
            }

            var player = GameObject.FindWithTag("Player");
            var nav = player.GetComponent<NavMeshAgent>();

            nav.enabled = false;
            player.transform.position = destinationPortal.portalSpawnPoint.position;
            player.transform.rotation = destinationPortal.portalSpawnPoint.rotation;
            nav.enabled = true;
        }

        private Portal GetDestinationPortal()
        {
            foreach (var portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return portal;
            }

            return null;
        }
    }
}
