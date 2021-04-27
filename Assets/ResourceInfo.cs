using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WorkerManager))]
public class ResourceInfo : MonoBehaviour
{
    public ResourceTypeInfo resourceTypeInfo;
    public  WorkerManager workerManager;
    private int factionId;

    public int FactionId
    {
        get => factionId;
        set => factionId = value;
    }

    public bool isEmpty;
    
    public bool IsEmpty
    {
        get => isEmpty;
        set => isEmpty = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        workerManager = GetComponent<WorkerManager>();
        EventCenter.Broadcast(EnumEventType.ResourceCreated,this);
    }

    public ResourceTypeInfo GetResourceType()
    {
        return resourceTypeInfo;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
