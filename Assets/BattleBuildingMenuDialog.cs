using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuDialogContext : DialogContext
{
   public BaseBattleBuilding targetUnitBase;
   public string[] menus;
   public int r = 120;

   public BuildingMenuDialogContext(BaseBattleBuilding targetUnitBase,string[] menus)
   {
      this.targetUnitBase = targetUnitBase;
      this.menus = menus;
   }
}
public class BattleBuildingMenuDialog : Dialog<BuildingMenuDialogContext>
{
   //此类为菜单dialog，内含多个按钮
   public Transform btnParent;
   public const string pathPrefix = "Prefab/UI/Menu/";
   public static Dialog ShowDialog(BaseBattleBuilding targetUnitBase, string[] menus)
   {
      var dialog = GetShowingDialog(nameof(BattleBuildingMenuDialog)) as BattleBuildingMenuDialog;
      if (dialog != null)
      {
         return null;
      }

      return DialogUtil.ShowDialogWithContext(nameof(BattleBuildingMenuDialog), new BuildingMenuDialogContext(targetUnitBase,menus));
   }

   public override void Show()
   {
      base.Show();
      string[] menus = dialogContext.menus;
      for (int i=0;i<menus.Length;i++)
      {
         //解析命令
         string[] splitedStrings = menus[i].Split('_');
         string command = splitedStrings[0].ToLower();
         string param0="";
         string param1="";
         if (splitedStrings.Length > 1)
         {
            param0 = splitedStrings[1].ToLower();
         }

         if (splitedStrings.Length > 2)
         {
            param1 = splitedStrings[2].ToLower();
         }
       
         switch (command)
         {
            case "spawn" : //生成单位
               InstantiateSpawnBtn(i,int.Parse(param0),float.Parse(param1));
               break;
            case "close" ://关闭菜单
               InstantiateCloseBtn(i);
               break;
         }
      }
   }

   private void InstantiateSpawnBtn(int index,int spawnId,float priceOff)//折扣，受各种buff、科技、政策影响
   {
      GameObject spawnPfb =
         Resources.Load<GameObject>(pathPrefix + "SpawnButton");
      GameObject iBtn = Instantiate(spawnPfb,GetBtnPosByIndex(index),Quaternion.identity,btnParent);
      
      iBtn.GetComponent<Button>().onClick.AddListener(()=>
      {
         dialogContext.targetUnitBase.AddUnitToSpawnStack(spawnId);
      });
   }
   
   private void InstantiateCloseBtn(int index)
   {
      GameObject closePfb =
         Resources.Load<GameObject>(pathPrefix +"CloseButton");
      GameObject iBtn = Instantiate(closePfb,GetBtnPosByIndex(index),Quaternion.identity,btnParent);
      iBtn.GetComponent<Button>().onClick.AddListener(Close);
   }

   private Vector3 GetBtnPosByIndex(int index)
   {
      Vector3 center = Camera.main.WorldToScreenPoint(dialogContext.targetUnitBase.transform.position);
      int total = dialogContext.menus.Length;
      int degree = 360 / total;
      Vector3 pos = center + new Vector3(dialogContext.r * Mathf.Sin(index * degree),
         dialogContext.r * Mathf.Cos(index * degree), 0);
      return pos;
   }
}
