using Mirror;
using MultiplayerRTS.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using MultiplayerRTS.Resources;
using TMPro;
using UnityEngine;

namespace MultiplayerRTS.UI
{
    public class ResourcesDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text resourcesText = null;
        [SerializeField] private TMP_Text resourcesInText = null;
        [SerializeField] private TMP_Text resourcesOutText = null;
        [SerializeField] private TMP_Text resourcesTotalText = null;
        private ResourceTracker _resourceTracker = null;

        private void Start()
        {
            try
            {
                _resourceTracker = NetworkClient.connection.identity.GetComponent<ResourceTracker>();
                if (_resourceTracker != null)
                {
                    _resourceTracker.OnResourcesDetails += HandleResourceDetails;
                    _resourceTracker.ClientOnResourceUpdate += ClientHandleResourceUpdate;
                    ClientHandleResourceUpdate(_resourceTracker.Resources);
                }
            }
            catch (Exception e)
            {
                try
                {
                    Debug.Log($"{NetworkClient.connection.identity.gameObject.name} {e.Message}");
                }
                catch
                {
                    Debug.Log($"{e.Message}");
                }

                return;
            }
        }

        private void HandleResourceDetails(float incoming, float outgoing, float total)
        {
            resourcesInText.text = $"In: {Mathf.Round(incoming).ToString()}";
            resourcesOutText.text = $"Out: {Mathf.Round(outgoing).ToString()}";
            resourcesTotalText.text = $"Total: {Mathf.Round(total).ToString()}";
        }

        private void OnDestroy()
        {
            _resourceTracker.OnResourcesDetails -= HandleResourceDetails;
            _resourceTracker.ClientOnResourceUpdate -= ClientHandleResourceUpdate;
        }

        private void ClientHandleResourceUpdate(int resource)
        {
            resourcesText.text = $"Resources: {resource.ToString()}";
        }
    }
}
