using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 玩家控制器脚本，继承自MonoBehaviour以实现Unity组件功能
public class PlayerScript : MonoBehaviour
{
    // [Header]属性用于在Inspector窗口中创建分组标题，提高代码可读性
    // "Player Movement"分组标题下的变量，用于控制玩家移动
    [Header("Player Movement")]
    public float playerSpeed = 1.9f;  // 玩家移动速度，默认值为1.9
    public float currentPlayerSpeed = 0f; // 玩家当前移动速度，默认值为0
    public float playerSprint = 3f; // 玩家冲刺速度，默认值为3f
    public float currentPlayerSprint = 0f; // 玩家当前冲刺速度，默认值为0

    [Header("玩家摄像机")]
    public Transform playerCamera;  // 玩家摄像机的Transform组件，用于获取摄像机的位置和旋转信息

    // "Player Animator and Gravity"分组标题下的变量，用于控制玩家动画和重力
    [Header("Player Animator and Gravity")]
    public CharacterController cC;  // 角色控制器组件，用于处理玩家移动和碰撞
    public float gravity = -9.81f;  // 重力加速度，默认值为-9.81，用于模拟玩家下落的效果

    [Header("Player Jumping & velocity")]
    public float jumpRange = 1f;  // 玩家跳跃范围，默认值为1f，用于模拟玩家跳跃的效果
    public float turnCalmTime = 0.1f;  // 玩家转向平滑时间，默认值为0.1秒
    float turnCalmVelocity;  // 玩家转向平滑速度，用于计算转向的平滑过渡
    Vector3 velocity;  // 玩家当前速度向量，用于计算玩家的移动和重力效果
    public Transform surfaceCheck;  // 玩家地面检测点的Transform组件，用于检测玩家是否在地面上
    bool onSurface;  // 玩家是否在地面上的布尔值，用于判断玩家是否需要重力加速度
    public float surfaceDistance = 0.4f;  // 玩家地面检测点的检测距离，默认值为0.4，用于判断玩家是否接触到地面
    public LayerMask surfaceMask;  // 地面层的LayerMask，用于检测玩家是否接触到地面

    void Update()
    {
        onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);  // 检测玩家是否接触到地面，如果接触到地面，则onSurface为true，否则为false

        if (onSurface && velocity.y < 0)
        {
            velocity.y = -2f;  // 玩家在地面上时，速度重置为-2f，模拟玩家跳跃的效果
        }
        
        velocity.y += gravity * Time.deltaTime;  // 计算重力加速度，根据重力加速度和时间间隔，计算玩家的下落速度
        cC.Move(velocity *Time.deltaTime);  // 使用角色控制器组件移动玩家，根据速度向量和时间间隔，计算玩家的移动距离

        playerMove();  // 调用playerMove方法处理玩家移动

        Jump();

        Sprint();
    }

    void playerMove()
    {
        float horizontal_axis = Input.GetAxis("Horizontal");  // 获取水平输入轴的值
        float vertical_axis = Input.GetAxisRaw("Vertical");  // 获取垂直输入轴的值

        Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis);  // 创建一个方向向量，表示玩家的移动方向

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y; // 计算目标角度，使用Atan2函数将方向向量转换为角度，并加上摄像机的旋转角度，使玩家面向移动方向
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);  // 平滑过渡当前角度到目标角度，使用SmoothDampAngle函数实现平滑旋转
            transform.rotation = Quaternion.Euler(0f, angle, 0f);  // 将玩家的旋转设置为目标角度，使玩家面向移动方向

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;  // 根据目标角度计算移动方向向量
            cC.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);  // 根据方向向量和移动速度，计算玩家的移动距离，并使用角色控制器组件进行移动
            currentPlayerSpeed = playerSpeed;
        }
    }

    void Jump()
    {
    // 检测是否按下跳跃键且角色在地面上
        if(Input.GetButtonDown("Jump") && onSurface)
        {
        // 使用物理公式计算初始跳跃速度
        // velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);
        // 这里的jumpRange应该是跳跃高度，gravity是重力加速度
           velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);   
        
        }
    }

    void Sprint()
    {
        if(Input.GetButton("Sprint") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) && onSurface)
        {
            float horizontal_axis = Input.GetAxis("Horizontal");  // 获取水平输入轴的值
            float vertical_axis = Input.GetAxisRaw("Vertical");  // 获取垂直输入轴的值

            Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis);  // 创建一个方向向量，表示玩家的移动方向

            if(direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y; // 计算目标角度，使用Atan2函数将方向向量转换为角度，并加上摄像机的旋转角度，使玩家面向移动方向
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);  // 平滑过渡当前角度到目标角度，使用SmoothDampAngle函数实现平滑旋转
                transform.rotation = Quaternion.Euler(0f, angle, 0f);  // 将玩家的旋转设置为目标角度，使玩家面向移动方向

                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;  // 根据目标角度计算移动方向向量
                cC.Move(moveDirection.normalized * playerSprint * Time.deltaTime);  // 根据方向向量和冲刺速度，计算玩家的移动距离，并使用角色控制器组件进行移动
                currentPlayerSprint = playerSprint;  // 更新玩家当前冲刺速度
            } 
        }
        
    }

    // 注释

}
