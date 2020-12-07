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
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    public void SetLookPos(Vector3 pos)
    {
        transform.position = pos + offset;
    }
}
