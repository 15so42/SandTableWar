using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Formations.Tasks
{
    public class NavMeshFormationGroup : FormationGroup
    {
        /// <summary>
        /// The NavMeshFormationAgent class contains component references and variables for each NavMeshAgent.
        /// </summary>
        public class NavMeshFormationAgent : FormationAgent
        {
            private NavMeshAgent navMeshAgent;

            public override float Speed { set { navMeshAgent.speed = value; } }
            public override float Radius { get { return navMeshAgent.radius; } }
            public override float RemainingDistance { get { return navMeshAgent.remainingDistance; } }
            public override float StoppingDistance { get { return navMeshAgent.stoppingDistance; } }
            public override bool HasPath { get { return navMeshAgent.hasPath; } }
            public override bool PathPending { get { return navMeshAgent.pathPending; } }
            public override bool AutoBreaking { set { navMeshAgent.autoBraking = value; } }

            /// <summary>
            /// Caches the component references and initialize default values.
            /// </summary>
            public NavMeshFormationAgent(Transform agent) : base(agent)
            {
                navMeshAgent = agent.GetComponent<NavMeshAgent>();

                if (navMeshAgent.hasPath) {
                    navMeshAgent.ResetPath();
                    navMeshAgent.isStopped = true;
                }
            }

            /// <summary>
            /// Resumes pathfinding.
            /// </summary>
            public override void Resume()
            {
                navMeshAgent.isStopped = false;
            }

            /// <summary>
            /// Sets the destination.
            /// </summary>
            public override void SetDestination(Vector3 destination)
            {
                destination.y = navMeshAgent.destination.y;
                if (navMeshAgent.destination != destination) {
                    navMeshAgent.SetDestination(destination);
                    navMeshAgent.isStopped = false;
                }
            }

            /// <summary>
            /// Rotates towards the target rotation.
            /// </summary>
            public override bool RotateTowards(Quaternion targetRotation)
            {
                if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f) {
                    return true;
                }
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, navMeshAgent.angularSpeed * Time.deltaTime);
                return false;
            }

            /// <summary>
            /// Stops the agent from moving.
            /// </summary>
            public override void Stop()
            {
                if (navMeshAgent.hasPath) {
                    navMeshAgent.isStopped = true;
                    navMeshAgent.ResetPath();
                }
            }

            /// <summary>
            /// The task has ended. Perform any cleanup.
            /// </summary>
            public override void End()
            {
                Stop();
                navMeshAgent.updateRotation = true;
                navMeshAgent.velocity = Vector3.zero;
            }
        }

        public override void OnAwake()
        {
            base.OnAwake();

            formationAgent = new NavMeshFormationAgent(transform);
        }

        public override void OnStart()
        {
            base.OnStart();

            if (leader.Value != null && leaderTree != null) {
                leaderAgent = new NavMeshFormationAgent(leaderTree.transform);
            }
        }

        protected override void AddAgentToGroup(Behavior agent, int index)
        {
            base.AddAgentToGroup(agent, index);

            if (leader.Value == null) {
                formationAgents.Insert(index, new NavMeshFormationAgent(agent.transform));
            }
        }
    }
}