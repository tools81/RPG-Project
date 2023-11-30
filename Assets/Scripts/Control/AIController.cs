using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameDevTV.Utils;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] bool guardLocation = true;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float suspicionPeriod = 8f; 
        [SerializeField] float dwellPeriod = 5f; 
        [SerializeField] float aggrevatePeriod = 5f;
        [Range(0,1)] 
        [SerializeField] float patrolSpeedFraction = 0.2f;    
        [SerializeField] float shoutDistance = 5f;

        GameObject player;
        Health health;
        Mover mover;
        Fighter fighter;
        ActionScheduler actionScheduler;
        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSpentDwellingAtWaypoint = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Awake() 
        {
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            actionScheduler = GetComponent<ActionScheduler>();

            guardPosition = new LazyValue<Vector3>(GetPosition);
        }

        private void Start() 
        {            
            guardPosition.value = GetPosition();
        }

        private void Update()
        {
            if (health.IsDead()) return;

            if (IsAggrevated() && fighter.CanAttack(player))
            {
                timeSinceLastSawPlayer = 0;
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionPeriod)
            {
                SuspicionBehaviour();
            }
            else if (patrolPath != null)
            {
                PatrolBehaviour();
            }
            else
            {
                GuardBehaviour();
            }

            UpdateTimers();
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSpentDwellingAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void GuardBehaviour()
        {
            if (guardLocation)
            {
                mover.MoveTo(guardPosition.value);
            }
        }

        private void SuspicionBehaviour()
        {
            actionScheduler.CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            fighter.Attack(player);
            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            var hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

            foreach (var hit in hits)
            {
                var ai = hit.collider.GetComponent<AIController>();
                if (ai == null) continue;

                ai.Aggrevate();
            }
        }

        private Vector3 GetPosition()
        {
            return transform.position;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;

            if (timeSpentDwellingAtWaypoint < dwellPeriod)
            {
                actionScheduler.CancelCurrentAction();
                return;
            }            

            if (AtWaypoint())
            {
                timeSpentDwellingAtWaypoint = 0;
                CycleWaypoint();
            }
            nextPosition = GetCurrentWaypoint();

            mover.StartMoveAction(nextPosition, patrolSpeedFraction);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypointPosition(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex, patrolPath.GetChildCount());
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        //Called by Unity
        private void OnDrawGizmosSelected() 
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        private bool IsAggrevated()
        {
            return Vector3.Distance(player.transform.position, transform.position) < chaseDistance ||
            timeSinceAggrevated < aggrevatePeriod;
        }
    }
}
