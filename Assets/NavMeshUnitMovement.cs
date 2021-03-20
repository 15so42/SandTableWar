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
        public void Start()
        {
            navMeshAgent =  GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
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
           if(overrideMovementCtrl==false || HasArrived() || navMeshAgent.isStopped)
               return;
           
           if (moveByAnim)
           {
               navMeshAgent.Move(refAnimDeltaPos * (moveByAnimSpeedMultiplier * Time.deltaTime));
           }
           else
           {
              
               if (navMeshAgent.remainingDistance < arriveDistance + 2)
               {
                   targetSpeed = 0;
               }
               else
               {
                   targetSpeed = extraRotateSpeed;
               }
            
               dampSpeed = Mathf.Lerp(dampSpeed, targetSpeed, Time.deltaTime*5f);
               
               navMeshAgent.Move(transform.forward * (dampSpeed * Time.deltaTime));
           }
        }

        
        private bool HasArrived()
        {
            float remainingDistance;
            if (navMeshAgent.pathPending) {
                remainingDistance = float.PositiveInfinity;
            } else {
                remainingDistance = navMeshAgent.remainingDistance;
            }

            return remainingDistance <= arriveDistance;
        }
    }
}