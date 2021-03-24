using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityTimer;


[TaskCategory("MyRTS")]
    public class CarveOrUnCarve : Action
    {
        public bool targetStatus;
        private NavMeshObstacle navMeshObstacle;

        private NavMeshAgent navMeshAgent;
        private int lastPriority=50;

        private int frameCount = 0;
        public override void OnAwake()
        {
            base.OnAwake();
            navMeshObstacle = GetComponent<NavMeshObstacle>(); 
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public override void OnStart()
        {
            lastPriority = navMeshAgent.avoidancePriority;
            frameCount = 0;
        }

        public override TaskStatus OnUpdate()
        {
            if (targetStatus)
            {
                GetComponent<NavMeshAgent>().enabled = !targetStatus;
                navMeshObstacle.enabled = targetStatus;
                navMeshObstacle.carving = targetStatus;
            }
            else
            {
                if (frameCount == 1)
                {
                    navMeshAgent.enabled = true;
                    return TaskStatus.Success;
                }

                navMeshObstacle.carving = false;
                navMeshObstacle.enabled = false;
                frameCount=1;
                return TaskStatus.Running;
            }

            return base.OnUpdate();
        }

        // IEnumerator EnableUnitMovementCoroutine()
        // {
        //     // navMeshObstacle.enabled = targetStatus;
        //     // navMeshObstacle.carving = targetStatus;
        //     // yield return new WaitForEndOfFrame();
        //     // GetComponent<NavMeshAgent>().enabled = !targetStatus;
        // }
    }
