using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Stats;
using RPG.Saving;
using System;

namespace RPG.Core.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] Health entityHealth;        
        [SerializeField] GameObject healthbar;
        [SerializeField] GameObject background;
        [SerializeField] Text healthText;
        private RectTransform rt;
        private float startWidth;
        private IStat health;
        private bool Visable = true;

        private void Awake()
        {
            StartHealthBar();
            health.onStatChange += UpdateUI;
        }
        private void StartHealthBar()
        {
            HealthBarEnable(true);
            health = entityHealth;
            rt = healthbar.GetComponent<RectTransform>();
            startWidth = GetComponent<RectTransform>().rect.width;            
        }
        private void UpdateUI()
        {
            bool isDead = false;
            try
            {
                isDead = entityHealth.IsDead();
                
            }
            catch (Exception e)
            {
                Debug.Log($"could not check health : {e.StackTrace}");
            }
            if (!Visable && !isDead)
            {
                //Debug.Log($"{entityHealth.gameObject.name} is ALIVE!!");
                Visable = true;
                HealthBarEnable(true);
            }
            if (!Visable) return;
            //Debug.Log($"{entityHealth.gameObject.name} dead = {isDead}");
            if (isDead)
            {
                //Debug.Log($"{entityHealth.gameObject.name} is DEAD!");
                Visable = false;
                HealthBarEnable(false);
                GetComponent<CanvasGroup>().alpha = 0;
            }
            float percent = health.PercentLeft();
            if (percent == 1f || percent == 0f)
            {
                GetComponent<CanvasGroup>().alpha = 0;
            }
            else
            {
                GetComponent<CanvasGroup>().alpha = 1;
            }
            SetHealthBar(WorkOutWidth(percent));
            healthText.text = Mathf.Round(health.CurrentValue()).ToString();
        }

        private void HealthBarEnable(bool enable)
        {
            //Debug.Log($"{entityHealth.name} healthbar has been set to {enable}");
            healthbar.SetActive(enable);
            healthText.enabled = enable;
            background.SetActive(enable);
        }

        private float WorkOutWidth(float percent)
        {
            //Debug.Log($"{entityHealth.name} health bar has been resized to (start width({startWidth}) - (startwidth({startWidth}) * percent({percent}) ) {startWidth - (startWidth * percent)}");
            return startWidth - (startWidth * percent);
        }

        public void SetHealthBar(float right)
        {
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, right, startWidth -right);
        }

        //public object CaptureState()
        //{
        //    return Visable;
        //}

        //public void RestoreState(object state)
        //{
        //    Debug.Log($"is visable is {Visable}");
        //    StartHealthBar();
        //}
    }
}
