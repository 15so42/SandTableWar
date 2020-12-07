using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEditor;
using UnityEngine;

public class FightingManager
{
    public BattleSceneState battleSceneState;
    public LogicMap logicMap;
    public List<BattleUnitBase> selectedUnits = new List<BattleUnitBase>(); //选中的单位

    private Camera mainCamera;
    private int campId;

    public void Init()
    {
        mainCamera = Camera.main;
        logicMap = Object.FindObjectOfType<LogicMap>();
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(GameManager.PLAYER_CAMP_ID, out var value))
        {
            campId = (int)value;
        };
       

    }

    public void SpawnBase()
    {
        //todo 暂时生成小兵
        GameObject spawnedBase=PhotonNetwork.Instantiate("BattleUnit/Solider1", logicMap.GetBasePosByPlayerId(campId), Quaternion.identity);
        spawnedBase.GetComponent<BattleUnitBase>().SetCampId(campId);
    }
    

    public void Update()
    {
        //鼠标控制
        if (Input.GetMouseButtonDown(0))
        {
            MouseClickHandle();
        }
    }

    public void MouseClickHandle()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, 999))
        {
            foreach (var unit in selectedUnits)
            {
                if (raycastHit.transform.gameObject.layer==LayerMask.NameToLayer("Ground"))
                {
                    unit.SetTargetPos(raycastHit.point);
                    GameObject mark=GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    mark.transform.position = raycastHit.point;
                }
            }
        }
    }

    public void SelectUnit(BattleUnitBase unitBase)
    {
        if (!selectedUnits.Contains(unitBase))
        {
            selectedUnits.Add(unitBase);
        }
    }
    
    public int CalDamage(int damage, int defense, DamageType damageType)
    {
        //使用英雄联盟的伤害计算公式
        return damage *(1 - (defense / (defense + 100)));
    }

    public void Attack(BattleUnitBase attcker,BattleUnitBase victim,int damageValue)
    {
       // Debug.Log($"单位{attcker.gameObject.name}对单位{victim.gameObject.name}找成了{damageValue}点伤害");
       victim.ReduceHp(damageValue);
    }
}