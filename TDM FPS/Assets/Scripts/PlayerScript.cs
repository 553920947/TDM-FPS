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

    // "Player Animator and Gravity"分组标题下的变量，用于控制玩家动画和重力
    [Header("Player Animator and Gravity")]
    public CharacterController cC;  // 角色控制器组件，用于处理玩家移动和碰撞

    [Header("Player Jumping & velocity")]
    public float turnCalmTime = 0.1f;  // 玩家转向平滑时间，默认值为0.1秒
    float turnCalmVelocity;  // 玩家转向平滑速度，用于计算转向的平滑过渡

    void Update()
    {
        playerMove();  // 调用playerMove方法处理玩家移动
    }

    void playerMove()
    {
        float horizontal_axis = Input.GetAxis("Horizontal");  // 获取水平输入轴的值
        float vertical_axis = Input.GetAxisRaw("Vertical");  // 获取垂直输入轴的值

        Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis);  // 创建一个方向向量，表示玩家的移动方向

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;  // 计算目标角度，将方向向量转换为角度   
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);  // 平滑过渡当前角度到目标角度，使用SmoothDampAngle函数实现平滑旋转
            transform.rotation = Quaternion.Euler(0f, angle, 0f);  // 将玩家的旋转设置为目标角度，使玩家面向移动方向
            cC.Move(direction.normalized * playerSpeed * Time.deltaTime);  // 根据方向向量和移动速度，计算玩家的移动距离，并使用角色控制器组件进行移动
        }
    }

}
