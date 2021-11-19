using UnityEngine;

namespace MultiplayerRTS.Resources
{
    [CreateAssetMenu(fileName = "Elements", menuName = "MultiplayerRTS/Elements/New Element", order = 0)]
    public class Element : ScriptableObject
    {
        [SerializeField] private string nameOfElement;
        [SerializeField] private string symbol;
        [SerializeField] private string description;
        [SerializeField] private float earthCrustPercentage;
    }
}