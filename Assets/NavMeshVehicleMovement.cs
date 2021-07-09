using System;
using BattleScene.Scripts;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityNavMeshAgent;
using UnityEngine;
using UnityEngine.AI;

namespace DefaultNamespace
{
    public class NavMeshVehicleMovement : MonoBehaviour
    {
        private NavMeshAgent navMeshAgent;

        private Animator animator;

        public bool overrideMovementCtrl;
        public bool moveByAnim;

        
        public bool isTurnRound;
        private bool hasSetTurnRoundDes = false;

        public float turnRadius = 4;
        public int turnDegreeStepValue = 120;//每次寻找路径点时的转弯角度
        public int turnAngleThreshold = 90;//大于这个角度就开始转弯

        public void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        // private Vector3 refAnimDeltaPos;
        // private void OnAnimatorMove()
        // {
        //     if (animator)
        //     {
        //         if (moveByAnim)
        //         {
        //             refAnimDeltaPos = animator.deltaPosition;
        //         }
        //     }
        // }
        
        
        private Vector3 realDest;//点击的真正路径点
        private Vector3 d1;//临时路径点
       

        public void Update()
        {

            if (overrideMovementCtrl == false || navMeshAgent.enabled == false || HasArrived() ||
                navMeshAgent.isStopped)
                return;


            Vector3 targetDir = navMeshAgent.destination - transform.position;
            if (Vector3.Angle(targetDir, transform.forward) > turnAngleThreshold && isTurnRound == false)//如果在背后   
            {
                realDest = navMeshAgent.destination;//在转弯开始时确定真正路径点，但是在转弯过程中如果玩家设定了新的目标点则会通过SetRealDest()函数更新真正目标点.
                isTurnRound = true;//只执行一次并开始转弯 
                navMeshAgent.autoBraking = false;//这是自动刹车的变量，不关闭的话就会有走一步停一步的感觉 
            }


            if (isTurnRound)
            {
                if (hasSetTurnRoundDes == false) //d1为第一个临时路径点
                {
                    d1 = FindTurnPoint(realDest);
                    BattleFxManager.Instance.SpawnFxAtPosInPhotonByFxType(BattleFxType.DestionMark,d1,Vector3.forward);// 显示路径点标志
                    hasSetTurnRoundDes = true;
                    navMeshAgent.SetDestination(d1);
                }

                if (Vector3.Distance(transform.position, d1) <= navMeshAgent.stoppingDistance*1.2f)//到达临时路径点后寻找下一个临时路径点
                {
                    if (Vector3.Angle(realDest - transform.position, transform.forward) > turnAngleThreshold)//注意对比角度是和真正路径点对比
                    {
                        d1 = FindTurnPoint(realDest);
                        BattleFxManager.Instance.SpawnFxAtPosInPhotonByFxType(BattleFxType.DestionMark,d1,Vector3.forward);
                        navMeshAgent.SetDestination(d1);
                    }
                }

                if (Vector3.Angle(realDest - transform.position, transform.forward) < turnAngleThreshold)//当角度小于90度时，结束转弯 
                {
                    navMeshAgent.SetDestination(realDest);
                    hasSetTurnRoundDes = false;
                    isTurnRound = false;
                    navMeshAgent.autoBraking = true;
                }
            }

            Vector3 FindTurnPoint(Vector3 realDest)//Find temporary turn point;
            {
                Vector3 direction = realDest - transform.position;
                var cross = Vector3.Cross(transform.forward, direction); //通过叉积来判断目标地点在左边还是右边来决定超哪边旋转
                
                Vector3 targetPos;
                NavMeshHit navMeshHit;
                if (cross.y < 0) //在左边，
                {
                    targetPos = transform.position - transform.right * turnRadius -
                                transform.right * (turnRadius * Mathf.Cos((180 - turnDegreeStepValue) * Mathf.Deg2Rad)) +
                                transform.forward * (turnRadius * Mathf.Sin((180 - turnDegreeStepValue) * Mathf.Deg2Rad));
                }
                else //在右边
                {
                    targetPos = transform.position + transform.right * turnRadius +
                                transform.right * (turnRadius * Mathf.Cos(turnDegreeStepValue * Mathf.Deg2Rad)) +
                                transform.forward * (turnRadius * Mathf.Sin(turnDegreeStepValue * Mathf.Deg2Rad));
                }

                //使用SamplePosition来保证找到的路径点再导航网格上， SamplePosition的半径不应太大，否则可能因为障碍物找到较远的位置导致奇怪的寻路路径
                
                if (NavMesh.SamplePosition(targetPos, out navMeshHit, 2f, -1))
                {
                    targetPos = navMeshHit.position; //找到在导航网格上的点
                }
                else //如果导航网格没有合适的目的地便直接朝realDest前进
                {
                    targetPos = this.realDest;
                }

                return targetPos;
            }
        }

        public void SetRealDest(Vector3 pos)//the real destination is the position you clicked;
        {
            realDest = pos;
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

            return remainingDistance <= navMeshAgent.stoppingDistance;
        }
    }
}