using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;


[TaskCategory("MyRTS")]
    public class CarveOrUnCarve : Action
    {
        public bool targetStatus;
        private NavMeshObstacle navMeshObstacle;
        
        public override void OnAwake()
        {
            base.OnAwake();
            navMeshObstacle = GetComponent<NavMeshObstacle>();
        }

        public override TaskStatus OnUpdate()
        {
            navMeshObstacle.carving = targetStatus;
            return base.OnUpdate();
        }
    }
