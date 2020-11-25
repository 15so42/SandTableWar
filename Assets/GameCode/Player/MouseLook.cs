using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity=100;
    private float mouseX;
    private float mouseY;
    private float xRotation=0;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //使PlayerBody绕世界Y轴旋转
        playerBody.transform.Rotate(Vector3.up * mouseX );
        //旋转相机
        xRotation -= mouseY ;
        xRotation = Mathf.Clamp(xRotation,-90, 90);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

    }
}
