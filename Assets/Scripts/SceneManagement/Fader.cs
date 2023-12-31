using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currenltyActiveFade = null;

        private void Awake() 
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(1f, time);
        }        

        public Coroutine FadeIn(float time)
        {
            return Fade(0f, time);
        }

        private Coroutine Fade(float target, float time)
        {
            if (currenltyActiveFade != null)
            {
                StopCoroutine(currenltyActiveFade);
            }

            currenltyActiveFade = StartCoroutine(FadeRoutine(target, time));
            return currenltyActiveFade;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }
    }
}