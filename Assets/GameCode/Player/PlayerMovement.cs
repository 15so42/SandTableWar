using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;
    public float speed=1;
    public float jumpHeight = 0.7f;


    //控制重力,初始y速度
    public Vector3 velocity;
    public float gravity = -9.8f;

    Vector3 move;
    Vector3 pointBottom;
    Vector3 pointTop;

    //落地检测
    [Header("=====落地检测器=====")]
    
    public bool isGrounded = true;
    public float overlapCapsuleOffset=1f;
    public float radiusMultiplier = 0.8f;
    float groundCheckRadius =0;
    [Header("落地忽略层级(如玩家本身)")]
    public LayerMask ignoreMask;


    // Start is called before the first frame update
    void Start()
    {
        
        characterController = GetComponent<CharacterController>();
        groundCheckRadius = characterController.radius*radiusMultiplier;
        
    }

    // Update is called once per frame
    void Update()
    {
        

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move = transform.right * x + transform.forward * z;
        characterController.Move(move*speed*Time.deltaTime);


        //落地检测
        //isGrounded = Physics.CheckSphere(groundChecker.position, groundCheckDistance, groundChecklayerMask);
        isGrounded = isOnGround();
        if (isGrounded && velocity.y < 0)//<0时表示正在下降
        {
            velocity.y = -2;
        }


        //跳跃
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }


        //积分思想
        
        velocity.y += gravity * Time.deltaTime;

        //这里需要额外乘一个velocity吗？velocity.y应该是速度才对啊？速度的话不应该是加速度乘时间吗？
        characterController.Move(velocity*Time.deltaTime);

    }

    bool isOnGround()
    {

        pointBottom = transform.position + transform.up * groundCheckRadius - transform.up * overlapCapsuleOffset;
        pointTop = transform.position + transform.up * characterController.height - transform.up * groundCheckRadius;
        

        Collider[] colliders = Physics.OverlapCapsule(pointBottom, pointTop, groundCheckRadius, ~ignoreMask);
        //for(int i = 0; i < colliders.Length; i++)
        //{
        //    Debug.Log(colliders[i]);
        //}
        if (colliders.Length != 0)
        {
            
            return true;
        }
        else
        {
            return false;
        }


    }

    
}
