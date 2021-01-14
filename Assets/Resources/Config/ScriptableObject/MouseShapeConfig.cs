using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Config/MouseShapeConfig")]
public class MouseShapeConfig : ScriptableObject
{
    public List<MouseShapePair> pairs;

    public Texture2D GetShapeByState(MouseState mouseState)
    {
        return pairs.Find(x => x.state == mouseState).texture2D;
    }

    public MouseShapeAlignment GetAlignment(MouseState mouseState)
    {
        return pairs.Find(x => x.state == mouseState).alignment;
    }

    public MouseShapePair GetMouseShapePair(MouseState mouseState)
    {
        return pairs.Find(x => x.state == mouseState);
    }
    
}

public enum MouseShapeAlignment
{
    UpperLeft,
    MiddleCenter
}
[Serializable]
public class MouseShapePair
{
    public MouseState state;
    public Texture2D texture2D;
    public MouseShapeAlignment alignment;
}
