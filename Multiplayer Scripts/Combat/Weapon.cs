using Mirror;
using MultiplayerRTS.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using MultiplayerRTS.Audio;
using UnityEditor;
using UnityEngine;

namespace MultiplayerRTS.Combat
{
    [RequireComponent(typeof(AudioSource))]
    public partial class Weapon : MonoBehaviour, ActiveSound
    {
        public event Action<ActiveSound> OnDeath;
        public enum WeaponSize { Small, Medium, Large}
        [SerializeField] FireGroup[] fireGroups = null;
        [SerializeField] GameObject projectilePrefab;
        [SerializeField] float fireRange = 20f;
        [SerializeField] float fireRate = 1f;
        [SerializeField] float rotaionSpeed = 100f;
        [SerializeField] int damage = 1;
        [SerializeField] WeaponSize weaponSize = WeaponSize.Medium;
        [SerializeField] Color gizmoColour = Color.white;
        private float lastFireTime;
        private AudioSource audioSource;
        public GameObject ProjectilePrefab { get => projectilePrefab;}
        public float FireRange { get => fireRange; }
        public float FireRate { get => fireRate; }
        public int Damage { get => damage;}
        private float lastFire = 0;
        private AudioDirector _audioDirector;
        public CombatTarget_RTS target = null;
        private Quaternion lookRotation;
        private Vector3 direction;
        [SerializeField] private AudioClip fireSound;

        public void Start()
        {
            audioSource = GetComponent<AudioSource>();
            _audioDirector = FindObjectOfType<AudioDirector>();
        }
        
        private void PlayFireSound()
        {
            //if(!_audioDirector.RequestPermissionToPlay(AudioEvent.Fire, this)) return;
            //_audioDirector.PlayingAudio(this, fireSound);
            if (audioSource.isPlaying) return;;
            audioSource.clip = fireSound;
            audioSource.Play();
        }
        
        // private void Update()
        // {
        //     if (target != null) RotateTowardsTarget();
        // }

        public void RotateTowardsTarget(Vector3 targetPos)
        {
            //find the vector pointing from our position to the target
            direction = (targetPos - transform.position).normalized;

            //create the rotation we need to be in to look at the target
            lookRotation = Quaternion.LookRotation(direction);

            //rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotaionSpeed);
        }

        public bool TryTarget(CombatTarget_RTS combatTarget)
        {
            if (lastFire < fireRate) return false;
            
            if (Vector3.Distance(combatTarget.AimAtPoint(transform.position).position, transform.position) > fireRange)
            {
                return false;
            }
            
            // if (!TargetInRange(combatTarget, transform.position, fireRange)) return false;
            combatTarget.onDestroy += ResetTarget;
            target = combatTarget;
            return true;
        }

        private static bool TargetInRange(CombatTarget_RTS combatTarget, Vector3 position, float range)
        {
            Collider[] hits = Physics.OverlapSphere(position, range);
            foreach (Collider hit in hits)
            {
                if (!hit.attachedRigidbody) continue;
                if (!hit.attachedRigidbody.TryGetComponent(out CombatTarget_RTS combatTargetHit)) continue;
                if (combatTarget == combatTargetHit)
                {
                    return true;
                }
            }
            return false;
        }

        private void ResetTarget(CombatTarget_RTS combatTarget)
        {
            combatTarget.onDestroy -= ResetTarget;
            if (combatTarget == target)
            {
                target = null;
            }
        }

        public GameObject GETGameObject()
        {
            return gameObject;
        }

        public void FireWeapon(CombatTarget_RTS target, NetworkConnection connectionToClient)
        {            
            //Debug.Log("Fire!");
            foreach(FireGroup fireGroup in fireGroups)
            {
                if (fireGroup.GetNextFirePosition(fireRange, target, connectionToClient, projectilePrefab, damage))
                    PlayFireSound();
            }      
            lastFire = 0;
        }

        private void OnDestroy()
        {
            OnDeath?.Invoke(this);
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = gizmoColour;
            Gizmos.DrawWireSphere(transform.position, fireRange);
        }

        
        public GameObject ActiveSoundGameObject()
        {
            return gameObject;
        }

        public AudioSource GETAudioSource()
        {
            return audioSource;
        }

        public bool HasAuthorityToPlay()
        {
            return true;
        }

        public void WorkOutTime(float deltaTime)
        {
            lastFire += deltaTime;
        }
    }
}
