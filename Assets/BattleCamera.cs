using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制相机移动，目前用单例编写，以后有必要时再优化
/// </summary>
public class BattleCamera : MonoBehaviour
{
    //PC端的操作和英雄联盟相同
    public float horThreshould = 0.9f;
    public float verThreshould = 0.9f;

    public float horSpeed = 2f;
    public float verSpeed = 2f;

    public static BattleCamera Instance;
    public Vector3 offset;
    private Camera mainCamera;

    [Header("框选")] public Material rectMat;
    public Color rectColor=Color.green;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        mainCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        #region 相机边缘移动
        Vector2 mousePos = Input.mousePosition;
        float horAxis = 0;
        float verAxis = 0;
        if (mousePos.x > Screen.width || mousePos.x < 0 || mousePos.y > Screen.height || mousePos.y < 0)
        {
            return;
        }
        if (mousePos.x > Screen.width * horThreshould)
            horAxis = 1;
        if (mousePos.x < Screen.width * (1 - horThreshould))
            horAxis = -1;
        if (mousePos.y > Screen.height * verThreshould)
            verAxis = 1;
        if (mousePos.y < Screen.height * (1 - verThreshould))
            verAxis = -1;
        //todo 斜向会较快，应保证速度相同
        transform.Translate((Vector3.right * (horAxis * horSpeed)+Vector3.forward * (verAxis * verSpeed))*Time.deltaTime,Space.World);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        

        #endregion

        #region 框选单位

        if (Input.GetMouseButtonDown(0))
        {
            if(UITool.IsPointerOverUIObject(Input.mousePosition))
                return;
            start = Input.mousePosition;
            isDrawingRectangle = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrawingRectangle = false;
            CheckRectUnit();
        }
        

        #endregion
    }

    //设置相机焦点
    public void SetLookPos(Vector3 pos)
    {
        transform.position = pos + offset;
    }
    
    
    //框选单位画线
    private Vector3 start;//框选开始位置
    private Vector3 end;
    private bool isDrawingRectangle;
    void OnPostRender()
    {//画线这种操作推荐在OnPostRender（）里进行 而不是直接放在Update，所以需要标志来开启
        if (isDrawingRectangle)
        {
            //Debug.Log("................");
            end = Input.mousePosition;//鼠标当前位置
            GL.PushMatrix();//保存摄像机变换矩阵,把投影视图矩阵和模型视图矩阵压入堆栈保存

            if (!rectMat)
               
                return;

            rectMat.SetPass(0);//为渲染激活给定的pass。

            GL.LoadPixelMatrix();//设置用屏幕坐标绘图

            GL.Begin(GL.QUADS);//开始绘制矩形

            GL.Color(new Color(rectColor.r, rectColor.g, rectColor.b, 0.1f));
            //设置颜色和透明度，方框内部透明

            //绘制顶点
            GL.Vertex3(start.x, start.y, 0);

            GL.Vertex3(end.x, start.y, 0);

            GL.Vertex3(end.x, end.y, 0);

            GL.Vertex3(start.x, end.y, 0);

            GL.End();


            GL.Begin(GL.LINES);//开始绘制线
            
            GL.Color(rectColor);//设置方框的边框颜色 边框不透明

            GL.Vertex3(start.x, start.y, 0);

            GL.Vertex3(end.x, start.y, 0);

            GL.Vertex3(end.x, start.y, 0);

            GL.Vertex3(end.x, end.y, 0);

            GL.Vertex3(end.x, end.y, 0);

            GL.Vertex3(start.x, end.y, 0);

            GL.Vertex3(start.x, end.y, 0);

            GL.Vertex3(start.x, start.y, 0);

            GL.End();

            GL.PopMatrix();//恢复摄像机投影矩阵

        }

    }

    //框选单位,所有单位放在BattleUnitBase静态列表中，框选时对比坐标即可
    private void CheckRectUnit()
    {
        float minX, maxX;
        float minY, maxY;
        if (start.x < end.x)
        {
            minX = start.x;
            maxX = end.x;
        }
        else
        {
            minX = end.x;
            maxX = start.x;
        }
        if (start.y < end.y)
        {
            minY = start.y;
            maxY = end.y;
        }
        else
        {
            minY = end.y;
            maxY = start.y;
        }


        FightingManager fightingManager = GameManager.Instance.GetFightingManager();
        foreach (var unit in BattleUnitBase.selfUnits)
        {
            Vector3 unitScreenPos = mainCamera.WorldToScreenPoint(unit.transform.position);
            if (unitScreenPos.x > minX && unitScreenPos.x < maxX && unitScreenPos.y > minY && unitScreenPos.y < maxY)
            {
                if (unit != null)
                { 
                    fightingManager.SelectUnit(unit);
                }
                
            }
        }
        
    }
}
