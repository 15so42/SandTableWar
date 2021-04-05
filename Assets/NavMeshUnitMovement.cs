using System;
using UnityEngine;
using UnityEngine.AI;

namespace DefaultNamespace
{
    public class NavMeshUnitMovement : MonoBehaviour
    {
        private NavMeshAgent navMeshAgent;
        
        private Animator animator;

        [Header("重写移动逻辑开关")]
        public bool overrideMovementCtrl;
        [Header("通过动画移动")]
        public bool moveByAnim;
        public float moveByAnimSpeedMultiplier=1;

        [Header("转弯速度,非载具一般设置为0")] public float extraRotateSpeed=0;
        [Header("必须和行为树里的arriveDistance相等")] 
        public float arriveDistance = 1;
        public AnimationCurve speedCurve;
        public float stoppingDistance;
        public float maxSpeed = 5;

        private Vector3 myDesiredVelocity;
        public void Start()
        {
            navMeshAgent =  GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            navMeshAgent.speed = 0;
            stoppingDistance = navMeshAgent.stoppingDistance;
        }

        private void OnAnimatorMove()
        {
            if (animator)
            {
                if (moveByAnim)
                {
                    refAnimDeltaPos = animator.deltaPosition;
                }
            }
           
        }
        
        private Vector3 refAnimDeltaPos;
        private float targetSpeed;
        private float dampSpeed;
        public void Update()
        {
           if(overrideMovementCtrl==false || navMeshAgent.enabled==false|| HasArrived() || navMeshAgent.isStopped )
               return;
           
           if (moveByAnim)
           {
               navMeshAgent.Move(refAnimDeltaPos * (moveByAnimSpeedMultiplier * Time.deltaTime));
           }
           else
           {
              
               // if (navMeshAgent.remainingDistance < arriveDistance + 2)
               // {
               //     targetSpeed = 0;
               // }
               // else
               // {
               //     targetSpeed = extraRotateSpeed;
               // }
               
               // if (navMeshAgent.hasPath)
               // {
               //     Vector3 corner=navMeshAgent.path.co
               // }
               targetSpeed = GetSpeedByDistance(navMeshAgent.remainingDistance);
               dampSpeed = Mathf.Lerp(dampSpeed, targetSpeed, Time.deltaTime*5f);
               
               navMeshAgent.Move(transform.forward * (dampSpeed * Time.deltaTime));
           }
        }

        private float GetSpeedByDistance(float distance)
        {
            float maxCurveValue=speedCurve.Evaluate(1);
            if (distance > stoppingDistance+1)//在距离stoppingDistance外一米就开始停止
            {
                return maxCurveValue*GetMaxSpeed();
            }
            else
            {
                return speedCurve.Evaluate(distance-1 / stoppingDistance);
                Debug.Log("In stoppingDistance");
            }
        }

        private float GetMaxSpeed()
        {
            //return navMeshAgent.speed;
            return maxSpeed;
        }
        
        private bool HasArrived()
        {
            float remainingDistance;
            if (navMeshAgent.pathPending) {
                remainingDistance = float.PositiveInfinity;
            } else {
                remainingDistance = navMeshAgent.remainingDistance;
            }

            return remainingDistance <= stoppingDistance;
        }
    }
}