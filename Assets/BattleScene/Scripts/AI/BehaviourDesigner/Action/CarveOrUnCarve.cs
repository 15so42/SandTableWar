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
        public override void OnAwake()
        {
            base.OnAwake();
            navMeshObstacle = GetComponent<NavMeshObstacle>(); 
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public override void OnStart()
        {
            lastPriority = navMeshAgent.avoidancePriority;
        }

        public override TaskStatus OnUpdate()
        {
            // if (targetStatus)
            // {
            //     GetComponent<NavMeshAgent>().enabled = !targetStatus;
            //     navMeshObstacle.enabled = targetStatus;
            //     navMeshObstacle.carving = targetStatus;
            // }
            // else
            // {
            //     StartCoroutine(EnableUnitMovementCoroutine());
            // }
            if (targetStatus)
            {
               navMeshAgent.avoidancePriority = 1;
            }
            else
            {
                navMeshAgent.avoidancePriority = 50;
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
