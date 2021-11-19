using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiplayerRTS.Stats;
using UnityEngine.UI;

namespace MultiplayerRTS.UI
{
    public class StatDisplayScript : MonoBehaviour
    {
        [SerializeField] IStat_RTS stat = null;
        [SerializeField] private GameObject iStat_RTS_GameObject = null;
        [SerializeField] private GameObject statBarParent = null;
        //[SerializeField] private GameObject statBarBackground = null;
        [SerializeField] private Image statBarImage = null;
        private bool isActive = false;
        private RectTransform rt;
        private float startWidth;

        private void Awake()
        {
            stat = iStat_RTS_GameObject.GetComponent<IStat_RTS>();
            if (stat != null) stat.ClientOnUpdateToStats += HandleStatUpdated;
            else Debug.Log("stat not set");
            rt = statBarImage.GetComponent<RectTransform>();
            startWidth = statBarImage.GetComponent<RectTransform>().rect.width;
        }

        private void OnDestroy()
        {
            if (stat != null) stat.ClientOnUpdateToStats -= HandleStatUpdated;
            else Debug.Log("stat not set");
        }

        private void OnMouseEnter()
        {
            //Debug.Log("Mouse Enter");
            //if (!mouseOverShow) return;
            if (!isActive)
            {
                statBarParent.SetActive(true);
                isActive = true;
            }
        }

        private void OnMouseExit()
        {
            //Debug.Log("Mouse Exit");
            //if (!mouseOverShow) return;
            if (isActive)
            {
                statBarParent.SetActive(false);
                isActive = false;
            }
        }

        private float WorkOutWidth(float percent)
        {
            //Debug.Log($"{entityHealth.name} health bar has been resized to (start width({startWidth}) - (startwidth({startWidth}) * percent({percent}) ) {startWidth - (startWidth * percent)}");
            return startWidth - (startWidth * percent);
        }

        private void HandleStatUpdated(int curentStat, int startStat)
        {
            float right = (float)curentStat / startStat;
            //Debug.Log($"HandleStatUpdated = {right}");
            right = WorkOutWidth(right);
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, right, startWidth - right);
            //statBarImage.fillAmount = (float)curentStat / startStat;
        }
    }
}
