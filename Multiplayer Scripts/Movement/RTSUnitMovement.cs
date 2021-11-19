using System;
using System.Collections;
using UnityEngine;
using Mirror;
using MultiplayerRTS.Audio;
using UnityEngine.AI;
using MultiplayerRTS.Combat;
using MultiplayerRTS.EndGame;


namespace MultiplayerRTS.Movement
{
    // ReSharper disable once InconsistentNaming
    public class RTSUnitMovement : NetworkBehaviour, PlaySoundEvent
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float timeAllowanceBeforeResetPath = 5f;
        [SerializeField] private CombatFighter_RTS combatFighter;
        [SerializeField] private float chaseRange = 10f;
        [SerializeField] private int voiceID = 0;
        public event Action<AudioEvent, int> onPlaySound;
        public event Action<PlaySoundEvent> onDeath;
        private bool _resetPathOnDestination;
        private float _timeStill;

        private Vector3 _targetDestination;
        
        private void TryWarp(Vector3 point)
        {
            FindWalkable(point, out var isWalkable, out var hit);
            if (!isWalkable || !GetComponent<NavMeshAgent>().Warp(hit.position))
            {
                //Debug.LogError($"{gameObject.name} mover could not be placed on nav mesh");
                if (GetComponent<NavMeshAgent>().Warp(point)) Debug.LogError($"{gameObject.name} " +
                                                                             $"2nd attempt mover placed on nav mesh");
                //else Debug.LogError($"{gameObject.name} 2nd attempt mover failed to place on nav mesh");
            }

        }

        public static bool CanMoveTo(Vector3 targetPosition, Vector3 currentPosition)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPoint = NavMesh.SamplePosition(targetPosition, out var hit, 1f, 
                NavMesh.AllAreas);
            if (!hasPoint)
            {
                Debug.Log("No Navmesh");
                return false;
            }
            bool hasPath = NavMesh.CalculatePath(currentPosition, hit.position, 
                NavMesh.AllAreas, path);
            return hasPath;
        }

        private static void FindWalkable(Vector3 point, out bool isWalkable, out NavMeshHit hit)
        {
            float y = point.y;
            if (Terrain.activeTerrain != null)
            {
                y = Terrain.activeTerrain.SampleHeight(point);

            }
            isWalkable = NavMesh.SamplePosition(new Vector3(point.x, y, point.z), out hit, 1f, 
                NavMesh.AllAreas);
            int attempts = 50;
            if (!isWalkable)
            {
                y = point.y;
                while (!isWalkable && attempts > 0)
                {
                    y -= .1f;
                    isWalkable = NavMesh.SamplePosition(new Vector3(point.x, y, point.z), out hit, 1f, 
                        NavMesh.AllAreas);
                    attempts -= 1;
                }
            }
        }

        #region Server

        public override void OnStartServer()
        {
            EndGameHandler.ServerOnGameOver += ServerHandleGameOver;
            TryWarp(transform.position);
        }

        public override void OnStopServer()
        {
            EndGameHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [ServerCallback]
        private void Update()
        {
            CombatTarget_RTS target = combatFighter.Target; 
            if (target != null)
            {
                Vector3 aim = target.AimAtPoint(transform.position).position;
                if ((aim - transform.position).sqrMagnitude > chaseRange * chaseRange)
                {
                    agent.SetDestination(aim);
                    return;
                }
                else if (agent.hasPath)
                {
                    ResetPath();
                }
            }
            _timeStill += Time.deltaTime;
            if (!agent.hasPath) return;
            if (agent.remainingDistance > agent.stoppingDistance  || 
                Vector3.Distance(transform.position, _targetDestination) <= agent.stoppingDistance)
            {
                _timeStill = 0f;
                return;
            }
            if (!_resetPathOnDestination) StartCoroutine(ResetPath(_targetDestination, 
                timeAllowanceBeforeResetPath));                     
        }

        private IEnumerator ResetPath(Vector3 destination, float time)
        {
            _resetPathOnDestination = true;
            //Debug.Log($"Try Path Reset in {time}");
            yield return new WaitForSeconds(time);
            //Debug.Log($"Trying Path Reset Now");
            if (_targetDestination == destination)
            {
                if (_timeStill > timeAllowanceBeforeResetPath)
                {
                    ResetPath();
                }
                else
                {
                    //Debug.Log($"Unit Has Moved Since Call Recalling Try Path Reset In
                    //{timeAllowanceBeforeResetPath - timeStill}");
                    StartCoroutine(ResetPath(destination, timeAllowanceBeforeResetPath - _timeStill));
                }
            }
            else
            {
                //if (targetDestination != destination) Debug.Log($"Destination Has Changed Cancelling Reset Path");
            }            
        }

        private void ResetPath()
        {
            //Debug.Log($"Path Has Been Reset");
            agent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 position)
        {
            ServerMove(position);
        }

        [Server]
        public void ServerMove(Vector3 position)
        {
            ResetPath();
            //Debug.Log($"Move Called Position {position.ToString()}");
            combatFighter.ClearTarget();
            _targetDestination = position;
            _timeStill = 0f;
            if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 50f, NavMesh.AllAreas)) return;
            agent.SetDestination(hit.position);
            PlaySound(AudioEvent.MoveOut);
            _resetPathOnDestination = false;
        }
        
        private void OnDestroy()
        {
            onDeath?.Invoke(this);
        }

        [Server]
        private void ServerHandleGameOver()
        {
            ResetPath();
        }

        #endregion

        #region Client
        [ClientRpc]
        private void PlaySound(AudioEvent audioEvent)
        {
            if (!hasAuthority) return;
            Debug.Log($"PLAY SOUND {audioEvent.ToString()}");
            onPlaySound?.Invoke(audioEvent, voiceID);
        }
        #endregion

        
    }
}
