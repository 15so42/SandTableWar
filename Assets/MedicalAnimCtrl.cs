using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using DG.Tweening;
using UnityEngine;

public class MedicalAnimCtrl : BattleUnitAnimCtrl
{

    public GameObject aidPacket;
    public Transform aidPacketSpawnPos;
    public void CureAnim()
    {
        anim.SetTrigger("Cure");
    }

    //帧事件
    public void CureAnimEvent()
    {
        GameObject cureBag = GameObject.Instantiate(aidPacket, aidPacketSpawnPos.position, Quaternion.identity);
        MedicalSolider medicalSolider = (battleUnitBase as MedicalSolider);
        if(medicalSolider.cureTarget==null)
            return;
        cureBag.transform.DOJump(medicalSolider.cureTarget.GetVictimPos(), 1, 1, 0.5f).OnComplete(
            () =>
            {
                medicalSolider.CureTargetUnit();
                cureBag.GetComponent<RecycleAbleObject>().Recycle();
            });
    }
}
