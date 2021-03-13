using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public enum SpawnBuildingTye
{
   res,//资源类建筑
   tactics//战术类建筑
}
public class SpawnBuildingDialogContext : DialogContext
{
    
}
public class SpawnBuildingDialog : Dialog<SpawnBuildingDialogContext>
{
   //建筑列表目前分为两类
   //已解锁的资源建筑
   public List<BuildingSpawnItem> resBuildings=new List<BuildingSpawnItem>();
   //已解锁的战术建筑
   public List<BuildingSpawnItem> tacticsBuildings=new List<BuildingSpawnItem>();

   public Transform resBuildingContainer;
   public Transform tacticsBuildingContainer;

   public Transform spawnBuildingItemTemplate;
   public SpawnBuildingTye lastOpenBuildingType;//记录上次打开的建筑栏
   public Button containerToggle;
   public Button resTypeButton;
   public Button tacticsTypeButton;
   private bool isShowContainer;//是否在显示整个建筑栏
   public static Dialog ShowDialog()
   {
      var dialog = GetShowingDialog(nameof(SpawnBuildingDialog)) as SpawnBuildingDialog;
      if (dialog != null)
      {
         return null;
      }

      return DialogUtil.ShowDialogWithContext(nameof(SpawnBuildingDialog), new SpawnBuildingDialogContext());
   }


   public override void Show()
   {
      base.Show();
      //初始化建筑栏
      Init();
      lastOpenBuildingType = SpawnBuildingTye.res;
      containerToggle.onClick.AddListener(OnToggleBtnClick);
      resTypeButton.onClick.AddListener(() =>
      {
         lastOpenBuildingType = SpawnBuildingTye.res;
         tacticsBuildingContainer.gameObject.SetActive(false);
         resBuildingContainer.gameObject.SetActive(true);
      });
      tacticsTypeButton.onClick.AddListener(() =>
      {
         lastOpenBuildingType = SpawnBuildingTye.tactics;
         tacticsBuildingContainer.gameObject.SetActive(true);
         resBuildingContainer.gameObject.SetActive(false);
      });
      
      AddSpawnBuildingItemByUnlock(SpawnBuildingTye.res, ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(BattleUnitId.Base));
   }

   //解锁可建造建筑时添加到对应的建造栏内
   public void AddSpawnBuildingItemByUnlock(SpawnBuildingTye buildingType,SpawnBattleUnitConfigInfo buildingInfo)
   {
      Transform itemGo = GameObject.Instantiate(spawnBuildingItemTemplate,
         buildingType == SpawnBuildingTye.res ? resBuildingContainer : tacticsBuildingContainer);
      itemGo.GetComponent<BuildingSpawnItem>().Init(buildingInfo);
      itemGo.gameObject.SetActive(true);
   }
   private void OnEnable()
   {
      Init();
   }

   private void Init()
   {
      UpdateResBuildingContainer();
      UpdateTacticsBuildingContainer();
   }

   private void Update()
   {
      UpdateResBuildingContainer();
      UpdateTacticsBuildingContainer();
   }

   private void UpdateResBuildingContainer()
   {
      foreach (var resBuildingItem in resBuildings)
      {
         resBuildingItem.UpdateSpawnBuildingItem();
      }
   }

   private void UpdateTacticsBuildingContainer()
   {
      foreach (var tacticsBuilding in tacticsBuildings)
      {
         tacticsBuilding.UpdateSpawnBuildingItem();
      }
   }

   public void OnToggleBtnClick()
   {
      //当前的状态，当前是关闭的，打开建筑栏
      if (isShowContainer == false)
      {
         isShowContainer = true;
      }
      else
      {//当前状态时开启的，本次点击关闭建筑栏
         tacticsBuildingContainer.gameObject.SetActive(false);
         resBuildingContainer.gameObject.SetActive(false);
         isShowContainer = false;
         return;
      }
      
      //当前是关闭的，打开时打开上一次浏览的建筑栏
      if (lastOpenBuildingType == SpawnBuildingTye.res)
      {
         tacticsBuildingContainer.gameObject.SetActive(false);
         resBuildingContainer.gameObject.SetActive(true);
      }
      else
      {
         tacticsBuildingContainer.gameObject.SetActive(true);
         resBuildingContainer.gameObject.SetActive(false);
      }
      
   }
   
}
