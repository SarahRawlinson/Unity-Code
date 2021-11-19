using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using MultiplayerRTS.Audio;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;


namespace MultiplayerRTS.FX
{
    [RequireComponent(typeof(HighlightEffect))]
    public class HighlightTrigger : MonoBehaviour
    {
        private void Start()
        {
            HighlightEffect highlighter = GetComponent<HighlightEffect>();
            if (highlighter != null)
            {
                HighlightOn(false);
                highlighter.highlighted = true;
                
            }
        }

        public void HighlightOn(bool on)
        {
            HighlightEffect highlighter = GetComponent<HighlightEffect>();
            if (highlighter != null)
            {
                if (on)
                {
                    highlighter.innerGlow = 5f;
                    highlighter.outline = 1f;
                }
                else
                {
                    highlighter.innerGlow = 0f;
                    highlighter.outline = 0f;
                }
            }
        }

        private void OnDestroy()
        {
            HighlightEffect highlighter = GetComponent<HighlightEffect>();
            if (highlighter != null)
            {
                highlighter.highlighted = false;
            }
        }
        
    }
}
