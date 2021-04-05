using System;
using BattleScene.Scripts;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityNavMeshAgent;
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



        private BattleUnitBase battleUnitBase;
        public bool isTurnRound;
        private bool setTrunRoundDes=false;

        private Vector3 myDesiredVelocity;
        public void Start()
        {
            battleUnitBase = GetComponent<BattleUnitBase>();
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


        private Vector3 lastDes;
        private Vector3 d1;
        private Vector3 d2;
        public void Update()
        {
            
            // if(navMeshAgent.destination)

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

               Vector3 targetDir = navMeshAgent.destination - transform.position;
               if (Vector3.Angle(targetDir, transform.forward) > 120 && isTurnRound==false)
               {
                   lastDes = navMeshAgent.destination;
                   isTurnRound = true;
                   navMeshAgent.autoBraking = false;
               }
              

               if (isTurnRound)
               {
                   if (setTrunRoundDes == false)
                   {
                       
                       d1 = transform.position + transform.right * 5 +
                           transform.right * (5 * Mathf.Cos(120*Mathf.Deg2Rad))+ transform.forward * (5 * Mathf.Sin(120*Mathf.Deg2Rad));
                       BattleFxManager.Instance.SpawnFxAtPosInPhotonByFxType(BattleFxType.DestionMark,d1,Vector3.forward);
                       d2=transform.position + transform.right * 5 +
                          transform.right * (5 * Mathf.Cos(60*Mathf.Deg2Rad)) + transform.forward * (5 * Mathf.Sin(60*Mathf.Deg2Rad));
                       BattleFxManager.Instance.SpawnFxAtPosInPhotonByFxType(BattleFxType.DestionMark,d2,Vector3.forward);
                       setTrunRoundDes = true;
                       battleUnitBase.SetTargetPos(d1);
                   }

                   if (Vector3.Distance(transform.position, d1) <1.1f)
                   {
                       battleUnitBase.SetTargetPos(d2);
                   }
                   if (Vector3.Distance(transform.position, d2)<1.1f )
                   {
                       battleUnitBase.SetTargetPos(lastDes);
                       setTrunRoundDes = false;
                       isTurnRound = false;
                       navMeshAgent.autoBraking = true;
                   }
                   
               }
               
                   
               // targetSpeed = GetSpeedByDistance(navMeshAgent.remainingDistance);
               // dampSpeed = Mathf.Lerp(dampSpeed, targetSpeed, Time.deltaTime*5f);
               //
               // navMeshAgent.Move(transform.forward * (dampSpeed * Time.deltaTime));
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