using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Config/LineConfig")]
public class LineConfig : ScriptableObject
{
    public List<LineConfigPair> lineConfigPairs=new List<LineConfigPair>();
   
    public GameObject GetLinePfbByLineMode(LineMode lineMode)
    {
        return lineConfigPairs.Find(x => x.lineMode == lineMode).lineRenderPfb;
    }
}

[System.Serializable]
public class LineConfigPair
{
    public LineMode lineMode;
    public GameObject lineRenderPfb;
}
