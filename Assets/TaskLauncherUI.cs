using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using UnityEngine;

/// <summary>
/// 用于显示TaskLauncher进度的UI
/// </summary>
public class TaskLauncherUI : MonoBehaviour
{
    [Header("ItemUI预制体")] [SerializeField] private GameObject taskLauncherItemPfb;
    private TaskLauncher taskLauncher;

    [SerializeField]private Transform horizontalGridParent;
    //private List<FactionEntityTask> tasksList;
    private Queue<TaskLauncherItemUI> taskLauncherItems=new Queue<TaskLauncherItemUI>();

    private Vector3 offset;

    private Camera mainCamera;
    private Transform taskLauncherTrans;
    
    // Start is called before the first frame update
    void Start()
    {
       // gameObject.SetActive(false);
    }

    public void Init(TaskLauncher taskLauncher1,Vector3 offset)
    {
        this.taskLauncher = taskLauncher1;
        this.offset = offset;
        mainCamera = Camera.main;
        taskLauncherTrans = taskLauncher1.transform;
        EventCenter.AddListener<TaskLauncher,int,int>(EnumEventType.OnTaskLaunched,OnTaskLaunched);
        EventCenter.AddListener<TaskLauncher,int>(EnumEventType.OnTaskCompleted,OnTaskCompleted);
        
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<TaskLauncher,int,int>(EnumEventType.OnTaskLaunched,OnTaskLaunched);
        EventCenter.RemoveListener<TaskLauncher,int>(EnumEventType.OnTaskCompleted,OnTaskCompleted);
    }

    private void UpdatePos()
    {
        transform.position = mainCamera.WorldToScreenPoint(taskLauncherTrans.position) + offset;
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdatePos();
        foreach (var itemUi in taskLauncherItems)
        {
            if(itemUi!=null)
                itemUi.UpdateItemUi();
        }
        
    }

    void OnTaskLaunched(TaskLauncher taskLauncher1, int taskId, int queueId)
    {
        if (taskLauncher1 == taskLauncher)
        {
            Add(taskLauncher1.GetTask(taskId),queueId);
        }
    }
    void OnTaskCompleted(TaskLauncher taskLauncher1, int taskId)
    {
        if (taskLauncher1 == taskLauncher)
        {
            Remove();
        }
    }
    
    public void Add(FactionEntityTask factionEntityTask,int queueId)
    {
        //tasksList.Add(factionEntityTask);
        taskLauncherItems.Enqueue(InstaniateItemUI(factionEntityTask,queueId));
    }

    public void Remove(FactionEntityTask factionEntityTask=null)
    {
        if (factionEntityTask == null)
        {
            Destroy(taskLauncherItems.Dequeue().gameObject);
        }
            
        // TaskLauncherItemUI itemUI = taskLauncherItems.Find(x => x.factionEntityTask == factionEntityTask);
        // Destroy(itemUI.gameObject);
    }
    
   

    private TaskLauncherItemUI InstaniateItemUI(FactionEntityTask factionEntityTask,int queueId)//queueId现在的需求暂时用不到，但是先留着吧
    {
        GameObject itemGo = Instantiate(taskLauncherItemPfb, horizontalGridParent);
        var itemUI= itemGo.GetComponent<TaskLauncherItemUI>();
        itemUI.Init(taskLauncher,factionEntityTask,queueId);
        return itemUI;
    }
}
