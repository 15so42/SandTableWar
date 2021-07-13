using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

/// <summary>
/// 行为树测试用例
/// </summary>
public class IsConditionTrue : Conditional
{
    public BehaviorTreeTest treeTest;

    public SharedBool bool1;
    public override TaskStatus OnUpdate()
    {
        return bool1.Value ? TaskStatus.Success : TaskStatus.Failure ;
    }
}
