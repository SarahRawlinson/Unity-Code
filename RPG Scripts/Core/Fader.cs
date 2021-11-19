using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Fader : MonoBehaviour
    {

        private CanvasGroup canvasGroup;
        private Coroutine activeFade;

        private void Start()
        {
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
        }


        public Coroutine Fade(float time, float target)
        {
            Debug.Log($"fade to {target}");
            if (activeFade != null)
            {
            StopCoroutine(activeFade);
            }
            StartCoroutine(FadeRoutine(time, target));
            Debug.Log($"fade to {target} Complete");
            return activeFade;
            
        }

        private IEnumerator FadeRoutine(float time, float target)
        {
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }

        public void FadeImmediate(float target)
        {
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = target;
        }

    }
}
