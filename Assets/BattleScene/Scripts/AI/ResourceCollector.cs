using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;


[System.Serializable]
    public struct CollectionObject
    {
        public ResourceTypeInfo resourceType; //the resource type that this collection object is associated with
        
    }

public struct CollectCapacityConfig
{
    public ResourceTypeInfo resourceTypeInfo;
    [Header("每次动作采集量")]
    public int collectSpeed;

    public int collectedAmount;
    public int weight;//重量

    //public int maxCollectCapacity;
}
    public class ResourceCollector : MonoBehaviour
    {
        public CollectionObject[] collectionObjects = new CollectionObject[0];
        //only assigned resource types to the above collectionObjects array can be collected by the unit with this component
        private Dictionary<ResourceTypeInfo, CollectionObject> collectionObjectsDic = new Dictionary<ResourceTypeInfo, CollectionObject>();
        public ResourceInfo target;
        private Action<ResourceInfo> onSetTargetAction;

        private bool collecting;
        public List<CollectCapacityConfig> collectCapacityConfigs=new List<CollectCapacityConfig>();
        private Dictionary<ResourceTypeInfo,CollectCapacityConfig> collectProgress=new Dictionary<ResourceTypeInfo, CollectCapacityConfig>();
        [SerializeField]
        private int totalWeight;

        public int maxTotalWeight;
        private void Start()
        {
            foreach (var config in collectCapacityConfigs)
            {
                collectProgress.Add(config.resourceTypeInfo, config);
            }
        }

        public bool CanCollectResourceType (ResourceTypeInfo resourceType, bool useDic = true)
        {
            if (useDic) //the dictionary can be only used if the source unit has already been initialized.
                return collectionObjectsDic.ContainsKey(resourceType);
            else
            {
                foreach (CollectionObject co in collectionObjects)
                    if (co.resourceType == resourceType)
                        return true;

                return false;
            }
        }

        public ResourceInfo GetTarget()
        {
            return target;
        }

        public void SetTarget(ResourceInfo value)
        {
            target = value;
            onSetTargetAction?.Invoke(value);
            EventCenter.Broadcast(EnumEventType.OnUnitCollectionOrder,GetComponent<BattleUnitBase>(),value);
        }

        private float timer;
        public void Update()
        {
            if (OverWeight())
            {
                StopCollect();
                return;
            }
            if(collecting)
            {
                timer += Time.deltaTime;
                if (timer > 1)
                {
                    CollectCapacityConfig config = collectProgress[target.resourceTypeInfo];
                    config.collectedAmount += config.collectSpeed;
                    totalWeight += config.collectedAmount*config.weight;
                    timer = 0;
                }
            }
        }

        public bool OverWeight()
        {
            return totalWeight >= maxTotalWeight;
        }

        public void StartCollect()
        {
            collecting = true;
        }

        public void StopCollect()
        {
            collecting = false;
        }

        public void RegisterSetTargetAction(Action<ResourceInfo> action)
        {
            onSetTargetAction += action;
        }
    }
