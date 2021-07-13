using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

public class BehaviorTreeTest : MonoBehaviour
{
    public BehaviorTree behaviorDesigner;

    public bool enableSpace;
    // Start is called before the first frame update
    void Start()
    {
        behaviorDesigner = GetComponent<BehaviorTree>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            behaviorDesigner.SetVariableValue("bool1",true);
        }
      
    }
}
