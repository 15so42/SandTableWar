using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseState
{
    Default,
    OnSelfUnit,
    OnAllyUnit,
    OnEnemyUnit,
}
public class MouseShapeManager : MonoBehaviour
{
    public MouseShapeConfig mouseShapeConfig;
    public MouseState curState;

    public static MouseShapeManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetMouseState(MouseState nextState)
    {
        if (curState != nextState)
        {
            curState = nextState;
            MouseShapePair pair = mouseShapeConfig.GetMouseShapePair(curState);
            Texture2D texture2D = pair.texture2D;
            MouseShapeAlignment alignment = pair.alignment;
            Vector2 offset=Vector2.zero;
            if (alignment == MouseShapeAlignment.MiddleCenter)
            {
                if (texture2D != null)
                {
                    offset = new Vector2(x:(float)texture2D.width / 2, (float)texture2D.height / 2);
                }
            }
            Cursor.SetCursor(texture2D, offset, CursorMode.Auto);
        }
        
    }

    public MouseState GetCurMouseState()
    {
        return curState;
    }
}
