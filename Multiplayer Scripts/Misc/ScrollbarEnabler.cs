using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MultiplayerRTS
{    
    public class ScrollbarEnabler : MonoBehaviour
    {
        [SerializeField]
        RectTransform container;
        [SerializeField]
        RectTransform content;
        [SerializeField]
        Scrollbar scrollbar;

        public bool enableScrollbar = false;

        void Update()
        {
            if (enableScrollbar != scrollbar.gameObject.activeSelf)
                scrollbar.gameObject.SetActive(enableScrollbar);
        }

        void OnRectTransformDimensionsChange()
        {
            enableScrollbar = container.rect.height < content.rect.height;
        }
    }
}
