using MultiplayerRTS.Control;
using MultiplayerRTS.Networking;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MultiplayerRTS.UI
{
    public class ControlItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] internal Image iconActive;
        [SerializeField] internal Image iconInactive;
        [SerializeField] internal TMP_Text priceText;
        public event Action<ControlItemButton> OnButtonActive;
        public event Action<ControlItemButton> OnEnterButton;
        public event Action<ControlItemButton> OnExitButton;
        public event Action<ControlItemButton> OnButtonDeactivate;
        public event Action<ControlItemButton> OnButtonDestroy;
        internal ControlItem ThisControlItem;
        internal RTSNetworkPlayer Player;

        public ControlItem StationOrUnit { get => ThisControlItem; set => ThisControlItem = value; }

        internal void Activate()
        {
            OnButtonActive?.Invoke(this);
        }

        internal void Deactivate()
        {
            OnButtonDeactivate?.Invoke(this);
        }

        private void OnDestroy()
        {
            OnButtonDestroy?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnEnterButton?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnExitButton?.Invoke(this);
        }
    }
}
