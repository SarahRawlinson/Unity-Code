using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace MultiplayerRTS.FX
{
    public class FXController : MonoBehaviour
    {
        [System.Serializable]
        class SpawnGameObjects
        {
            [SerializeField] private GameObject[] gameObjects;
            [SerializeField] private float destroyAfter = 50f;

            public List<GameObject> CreateObjects(Vector3 position)
            {
                List<GameObject> fxInstances = new List<GameObject>();
                foreach (GameObject gameObject in gameObjects)
                {
                    GameObject fxInstance = Instantiate(gameObject, position, quaternion.identity);
                    fxInstances.Add(fxInstance);
                    DestroyAfterTime destroyAfterTime = fxInstance.AddComponent(typeof(DestroyAfterTime)) 
                        as DestroyAfterTime;
                    destroyAfterTime.DestroyMe(destroyAfter);
                }
                return fxInstances;
            }
        }

        [SerializeField] private SpawnGameObjects[] spawnGroupOnDeath;

        private void OnDestroy()
        {
            foreach (SpawnGameObjects spawnGroup in spawnGroupOnDeath)
            {
                spawnGroup.CreateObjects(transform.position);
            }
        }
    }
}
