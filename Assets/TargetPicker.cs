using System.Collections;
using System.Collections.Generic;
using RTSEngine;
using UnityEngine;

[System.Serializable]
public abstract class TargetPicker<T, V>
{
    [SerializeField, Tooltip("Defines the potential targets.")]
    protected List<V> targetList = new List<V>();

    /// <summary>
    /// Determines whether a target 't' can be picked as a valid target.
    /// </summary>
    /// <param name="t">The target to test its validity.</param>
    /// <returns>ErrorMessage.none if the target 't' can be picked, otherwise ErrorMessage.invalidTarget.</returns>
    public virtual ErrorMessage IsValidTarget (T t)
    {
        return ErrorMessage.none;
    }

    
}
