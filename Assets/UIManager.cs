using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{

   private Camera mainCam;
   
   private Transform mainCanvas;
   private Transform taskLauncherUIParent;//程序生成

   [Header("预制体")] public GameObject taskLauncherUIPfb;
 

   private void Start()
   {
      mainCanvas = GameObject.Find("Canvas").transform;
      
      taskLauncherUIParent=new GameObject("taskLauncherUIParent").transform;
      taskLauncherUIParent.SetParent(mainCanvas);
      
      mainCam=Camera.main;
      
   }

   public TaskLauncherUI SetTaskLauncherUI(GameObject targetObj,Vector3 offset)
   {
      GameObject uiGO = Instantiate(taskLauncherUIPfb, taskLauncherUIParent);
      uiGO.transform.localPosition=mainCam.WorldToScreenPoint(targetObj.transform.position);
      TaskLauncherUI taskLauncherUi = uiGO.transform.GetComponent<TaskLauncherUI>();

      TaskLauncher taskLauncher = targetObj.GetComponent<TaskLauncher>();
      if (taskLauncher)
      {
         taskLauncherUi.Init(taskLauncher,offset,GameManager.Instance.GetFightingManager());
      }

      return taskLauncherUi;

   }
   
  
}
