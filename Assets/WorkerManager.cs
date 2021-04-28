using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorkerManager : MonoBehaviour
{
    public List<ResourceCollector> workers=new List<ResourceCollector>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanAddWorker()
    {
        return workers.Count < GetAvailableSlots();
    }

    public void Add(ResourceCollector resourceCollector)
    {
        if(CanAddWorker())
            workers.Add(resourceCollector);
        currentWorker = workers.Count;
    }

    public void Remove(ResourceCollector resourceCollector)
    {
        workers.Remove(resourceCollector);
        currentWorker = workers.Count;
    }

    public int currentWorker ;
    //todo 
    public int GetAvailableSlots()
    {
        return 1;
    }
}
