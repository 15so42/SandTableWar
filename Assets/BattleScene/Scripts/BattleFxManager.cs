﻿using System;
using Photon.Pun;
using UnityEngine;
using Object = System.Object;

namespace BattleScene.Scripts
{
    public class BattleFxManager : MonoBehaviour
    {
        public static BattleFxManager Instance;
        private PhotonView photonView;
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            photonView=PhotonView.Get(this);
        }

        public const string fxPath = "Prefab/Fx/";
        [PunRPC]
        private void SpawnFxAtPos(string fxName,Vector3 pos,Vector3 forward)
        {
            RecycleAbleObject recycleAbleObject = UnityObjectPoolManager.Allocate(fxName);//对象池
            GameObject fxGo;
            if (recycleAbleObject==null)
            {
                fxGo= GameObject.Instantiate(Resources.Load<GameObject>(fxPath + fxName));
            }
            else
            {
                fxGo = recycleAbleObject.gameObject;
            }

            fxGo.transform.position = pos;
            fxGo.transform.forward = forward;
            
        }

        public void SpawnFxAtPosInPhoton(string fxName,Vector3 pos,Vector3 forward)
        {
            photonView.RPC(nameof(SpawnFxAtPos),RpcTarget.All,new Object[]{fxName,pos,forward});
        }
    }
}