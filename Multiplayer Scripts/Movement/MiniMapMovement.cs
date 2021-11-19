using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using MultiplayerRTS.Networking;
using UnityEngine.EventSystems;

namespace MultiplayerRTS.Movement
{
    public class MiniMapMovement : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private RectTransform miniMapRect = null;
        [SerializeField] private float mapScale = 1000f;
        [SerializeField] private float offset = -6;
        private Transform playerCameraTransform = null;

        private void Update()
        {
            if (playerCameraTransform != null) return;
            try
            {
                if (NetworkClient.connection.identity == null)
                {
                    return;
                }
                playerCameraTransform = NetworkClient.connection.identity.GetComponent<RTSNetworkPlayer>().CameraTransform;
            }
            catch
            {

            }
            
        }
        private void MoveCamera()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapRect, mousePos, null, out Vector2 localPos))
            {
                return;
            }
            Vector2 lerp = new Vector2((localPos.x - miniMapRect.rect.x) / miniMapRect.rect.width, (localPos.y - miniMapRect.rect.y) / miniMapRect.rect.height);
            var position = playerCameraTransform.position;
            Vector3 newCamera = new Vector3(Mathf.Lerp(-mapScale, mapScale, lerp.x), position.y, Mathf.Lerp(-mapScale, mapScale,lerp.y) - position.y);
            position = newCamera + new Vector3(0f, 0f, offset);
            playerCameraTransform.position = position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            MoveCamera();
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveCamera();
        }
    }
}
