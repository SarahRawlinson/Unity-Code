using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Effects
{
    public class EffectsOnCollision : MonoBehaviour
    {
        [SerializeField] GameObject DeathFX;
        [SerializeField] Transform Parent;



        // Update is called once per frame
        void Update()
        {

        }



        void OnParticleCollision(GameObject other)
        {
            GameObject fx = Instantiate(DeathFX, transform.position, Quaternion.identity);
            fx.transform.parent = Parent;
            Invoke("DestroyEnemy", 0.2f);
            //DestroyEnemy();
        }

        private void DestroyEnemy()
        {
            Destroy(gameObject);
        }

        //private void AddCollider()
        //{
        //    Collider collider = gameObject.AddComponent<BoxCollider>();
        //    collider.isTrigger = false;
        //}
    }
}
