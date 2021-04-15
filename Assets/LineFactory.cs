using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LineMode
{
    Dotted,
    Solid
}
public class LineFactory : MonoBehaviour
{
    public static LineFactory Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public LineRenderer GetLineByLineMode(LineMode lineMode)
    {
        return GameObject.Instantiate(ConfigHelper.Instance.GetLinePfbByLineMode(lineMode)).GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
