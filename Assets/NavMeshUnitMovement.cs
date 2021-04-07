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

        [Header("重写移动逻辑开关")] public bool overrideMovementCtrl;
        [Header("通过动画移动")] public bool moveByAnim;
        public float moveByAnimSpeedMultiplier = 1;

        [Header("转弯速度,非载具一般设置为0")] public float extraRotateSpeed = 0;
        [Header("必须和行为树里的arriveDistance相等")] public float arriveDistance = 1;
        public AnimationCurve speedCurve;
        public float stoppingDistance;
        public float maxSpeed = 5;


        private BattleUnitBase battleUnitBase;
        public bool isTurnRound;
        private bool setTrunRoundDes = false;

        public float turnRadius = 4;
        public int turnDegree = 120;

        private Vector3 myDesiredVelocity;

        public void Start()
        {
            battleUnitBase = GetComponent<BattleUnitBase>();
            navMeshAgent = GetComponent<NavMeshAgent>();
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

            if (overrideMovementCtrl == false || navMeshAgent.enabled == false || HasArrived() ||
                navMeshAgent.isStopped)
                return;

            if (moveByAnim)
            {
                navMeshAgent.Move(refAnimDeltaPos * (moveByAnimSpeedMultiplier * Time.deltaTime));
            }
            else
            {
                Vector3 targetDir = navMeshAgent.destination - transform.position;
                if (Vector3.Angle(targetDir, transform.forward) > 90 )
                {
                    lastDes = navMeshAgent.destination;
                    isTurnRound = true;
                    navMeshAgent.autoBraking = false;
                }


                if (isTurnRound)
                {
                    if (setTrunRoundDes == false)
                    {
                        d1 = FindTurnPoint(lastDes);
                        setTrunRoundDes = true;
                        battleUnitBase.SetTargetPos(d1);
                    }

                    if (Vector3.Distance(transform.position, d1) < 1.1f)
                    {
                        if (Vector3.Angle(lastDes - transform.position, transform.forward) > 90)
                        {
                            d1 = FindTurnPoint(lastDes);
                            battleUnitBase.SetTargetPos(d1);
                        }
                    }

                    if (Vector3.Angle(lastDes - transform.position, transform.forward) < 90)
                    {
                        battleUnitBase.SetTargetPos(lastDes);
                        setTrunRoundDes = false;
                        isTurnRound = false;
                        navMeshAgent.autoBraking = true;
                    }
                }
            }

            Vector3 FindTurnPoint(Vector3 lastDest)
            {
                Vector3 direction = lastDest - transform.position;
                var cross = Vector3.Cross(transform.forward, direction); //通过叉积来判断目标地点在左边还是右边来决定超哪边旋转
                Vector3 targetPos;
                NavMeshHit navMeshHit;
                if (cross.y < 0) //在左边，left side
                {
                    targetPos = transform.position - transform.right * turnRadius -
                                transform.right * (turnRadius * Mathf.Cos((180 - turnDegree) * Mathf.Deg2Rad)) +
                                transform.forward * (turnRadius * Mathf.Sin((180 - turnDegree) * Mathf.Deg2Rad));
                }
                else //在右边,right side
                {
                    targetPos = transform.position + transform.right * turnRadius +
                                transform.right * (turnRadius * Mathf.Cos(turnDegree * Mathf.Deg2Rad)) +
                                transform.forward * (turnRadius * Mathf.Sin(turnDegree * Mathf.Deg2Rad));
                }

                if (NavMesh.SamplePosition(targetPos, out navMeshHit, 2f, -1)
                ) //SamplePosition的半径不应太大，否则可能因为障碍物找到较远的位置导致奇怪的寻路路径
                {
                    targetPos = navMeshHit.position; //找到在导航网格上的点
                }
                else //cant find valid pos on NavMesh,return current transform.position
                {
                    targetPos = transform.position;
                }

                return targetPos;
            }
        }

        private float GetSpeedByDistance(float distance)
            {
                float maxCurveValue = speedCurve.Evaluate(1);
                if (distance > stoppingDistance + 1) //在距离stoppingDistance外一米就开始停止
                {
                    return maxCurveValue * GetMaxSpeed();
                }
                else
                {
                    return speedCurve.Evaluate(distance - 1 / stoppingDistance);
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
                if (navMeshAgent.pathPending)
                {
                    remainingDistance = float.PositiveInfinity;
                }
                else
                {
                    remainingDistance = navMeshAgent.remainingDistance;
                }

                return remainingDistance <= stoppingDistance;
            }
        }
    }