using System;
using System.Collections.Generic;
using UnityEngine;


    [System.Serializable]
    public struct CollectionObject
    {
        public ResourceTypeInfo resourceType; //the resource type that this collection object is associated with
        
    }
    public class ResourceCollector : MonoBehaviour
    {
        public CollectionObject[] collectionObjects = new CollectionObject[0];
        //only assigned resource types to the above collectionObjects array can be collected by the unit with this component
        private Dictionary<ResourceTypeInfo, CollectionObject> collectionObjectsDic = new Dictionary<ResourceTypeInfo, CollectionObject>();
        private ResourceInfo target;
        private Action<ResourceInfo> onSetTargetAction;
        
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
            target.workerManager.Add(this);
            onSetTargetAction?.Invoke(value);
        }

        public void RegisterSetTargetAction(Action<ResourceInfo> action)
        {
            onSetTargetAction += action;
        }
    }
