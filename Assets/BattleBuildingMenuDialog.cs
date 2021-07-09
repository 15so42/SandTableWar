using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildingMenuDialogContext : DialogContext
{
   public BaseBattleBuilding targetUnitBase;
   public BuildingMenuCommand[] menus;
   public int r = 240;

   public BuildingMenuDialogContext(BaseBattleBuilding targetUnitBase,BuildingMenuCommand[] menus)
   {
      this.targetUnitBase = targetUnitBase;
      this.menus = menus;
   }
}
public class BattleBuildingMenuDialog : Dialog<BuildingMenuDialogContext>
{
   //此类为菜单dialog，内含多个按钮
   public Transform btnParent;
   public const string pathPrefix = "Prefab/UI/BuildingMenuItem/";
   private List<BuildingMenuItem> buildingMenuItems=new List<BuildingMenuItem>();
   private Dictionary<GameObject,Vector3> itemsGo=new Dictionary<GameObject, Vector3>();

   private Camera mainCamera;
   public static Dialog ShowDialog(BaseBattleBuilding targetUnitBase, BuildingMenuCommand[] menus,Action closeAction)
   {
      var dialog = GetShowingDialog(nameof(BattleBuildingMenuDialog)) as BattleBuildingMenuDialog;
      if (dialog != null)
      {
         return null;
      }
      
      return DialogUtil.ShowDialogWithContext(nameof(BattleBuildingMenuDialog), new BuildingMenuDialogContext(targetUnitBase,menus),null,closeAction);
   }
   

   public override void Show()
   {
      hasAnim = false;
      mainCamera = Camera.main;
      base.Show();
      BuildingMenuCommand[] menus = dialogContext.menus;

      TaskLauncher taskLauncher = dialogContext.targetUnitBase.GetComponent<TaskLauncher>();
      for (int i=0;i<menus.Length;i++)
      {
         //解析命令
         BuildingMenuCommand buildingMenuCommand = menus[i];
         BuildingOperateType command = buildingMenuCommand.buildingOperateType;
         
         switch (command)
         {
            case BuildingOperateType.Spawn : //生成单位
               InstantiateSpawnBtn(i,taskLauncher,buildingMenuCommand.taskIndex,buildingMenuCommand.priceOff);
               break;
            case BuildingOperateType.SetSpawnPos : //进入修改出生点位置模式
               BuildingSetSpawnPosMenuItem buildingSetSpawnPosMenuItem = InstantiateCommonBtn(i, "SetSpawnPosButton",
                  () =>
                  {
                     dialogContext.targetUnitBase.StartSetSpawnPos();
                     GameManager.Instance.GetFightingManager().EnableSelectUnitByRect(false);
                     Close();
                  }) as BuildingSetSpawnPosMenuItem;
               buildingSetSpawnPosMenuItem?.SetParams(dialogContext.targetUnitBase, this);
               break;
            case BuildingOperateType.Close ://关闭菜单
               InstantiateCloseBtn(i);
               break;
            case BuildingOperateType.OutBuilding : //离开房屋
               InstantiateOutBuildingBtn(i);
               break;
         }
      }
   }

   private void InstantiateSpawnBtn(int index,TaskLauncher taskLauncher,int taskIndex,float priceOff)//折扣，受各种buff、科技、政策影响
   {
      GameObject spawnPfb =
         Resources.Load<GameObject>(pathPrefix + "SpawnButton");
      GameObject iBtn = Instantiate(spawnPfb,GetBtnPosByIndex(index),Quaternion.identity,btnParent);
      PlayScaleAnim(iBtn);
      
      
      iBtn.GetComponent<Button>().onClick.AddListener(()=>
      {
         dialogContext.targetUnitBase.taskLauncherComp.Add(taskIndex);
      });
      BuildingMenuItem item = iBtn.GetComponent<BuildingMenuItem>();
      if (item)
      {
         (item as BuildingSpawnMenuItem)?.SetParams(taskLauncher.GetTask(taskIndex),dialogContext.targetUnitBase);
         item.Init();
         buildingMenuItems.Add(item);
      }
      itemsGo.Add(iBtn,GetOffsetByIndex(index));
   }
   
   /// <summary>
   /// 生成关闭按钮
   /// </summary>
   /// <param name="index"></param>
   private void InstantiateCloseBtn(int index)
   {
      GameObject closePfb =
         Resources.Load<GameObject>(pathPrefix +"CloseButton");
      GameObject iBtn = Instantiate(closePfb,GetBtnPosByIndex(index),Quaternion.identity,btnParent);
      PlayScaleAnim(iBtn);
      iBtn.GetComponent<Button>().onClick.AddListener(Close);
      BuildingMenuItem item = iBtn.GetComponent<BuildingMenuItem>();
      if (item)
      {
         buildingMenuItems.Add(item);
      }
      itemsGo.Add(iBtn,GetOffsetByIndex(index));
   }

   private BuildingMenuItem InstantiateCommonBtn(int index,string pfbName,UnityAction action)
   {
      GameObject tmpPfb =
         Resources.Load<GameObject>(pathPrefix +pfbName);
      GameObject iBtn = Instantiate(tmpPfb,GetBtnPosByIndex(index),Quaternion.identity,btnParent);
      PlayScaleAnim(iBtn);
      iBtn.GetComponent<Button>().onClick.AddListener(action);
      BuildingMenuItem item = iBtn.GetComponent<BuildingMenuItem>();
      if (item)
      {
         buildingMenuItems.Add(item);
      }
      itemsGo.Add(iBtn,GetOffsetByIndex(index));
      return item;
   }
   

   private void InstantiateOutBuildingBtn(int index)
   {
      GameObject outBuildingPfb =
         Resources.Load<GameObject>(pathPrefix + "OutBuildingButton");
      
      GameObject iBtn = Instantiate(outBuildingPfb,GetBtnPosByIndex(index),Quaternion.identity,btnParent);
      PlayScaleAnim(iBtn);
      
      iBtn.GetComponent<Button>().onClick.AddListener(()=>
      {
         (dialogContext.targetUnitBase as DefenceBuilding)?.OutBuilding();
      });
      BuildingMenuItem item = iBtn.GetComponent<BuildingMenuItem>();
      if (item)
      {
         item.Init();
         buildingMenuItems.Add(item);
      }
      itemsGo.Add(iBtn,GetOffsetByIndex(index));
   }

   public void PlayScaleAnim(GameObject iBtn)
   {
      iBtn.transform.localScale=Vector3.zero;
      var sequence = DOTween.Sequence();
      sequence.Append(iBtn.transform.DOScale(Vector3.one, 0.5f)).SetEase(Ease.OutBounce);
   }
   

   private Vector3 GetBtnPosByIndex(int index)
   {
      Vector3 center = Camera.main.WorldToScreenPoint(dialogContext.targetUnitBase.transform.position);
      Vector3 offset = GetOffsetByIndex(index);
      Vector3 pos = center + offset;
      return pos;
   }

   private Vector3 GetOffsetByIndex(int index)
   {
      int total = dialogContext.menus.Length;
      int degree = 360 / total;
      Vector3 offset = new Vector3(dialogContext.r * Mathf.Sin(index * degree*Mathf.Deg2Rad),
         dialogContext.r * Mathf.Cos(index * degree*Mathf.Deg2Rad), 0);
      return offset;
   }
   public void Update()
   {
      foreach (var buildingMenuItem in buildingMenuItems)
      {
         buildingMenuItem.Update();
      }

      //同步位置
      Vector3 center=Camera.main.WorldToScreenPoint(dialogContext.targetUnitBase.transform.position);
      foreach (var kv in itemsGo)
      {
         kv.Key.transform.position = center + kv.Value;
      }
   }
}

public enum BuildingOperateType
{
   Spawn,
   Close,
   OutBuilding,
   SetSpawnPos
}
[System.Serializable]
public class BuildingMenuCommand
{
   public BuildingOperateType buildingOperateType;
   public int taskIndex;
   public float priceOff = 1;//折扣
}


