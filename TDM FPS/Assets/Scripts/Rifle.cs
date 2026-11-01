using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// 步枪脚本：挂在玩家枪械物体上，负责监听开火输入、发射射线检测命中的目标并造成伤害
public class Rifle : MonoBehaviour
{
    [Header("Rifle")] 
    public Camera cam; // 相机引用：用于获取射击起点（相机位置）和射击方向（相机前方）；可在Inspector手动赋值，也可留空由代码自动解析当前激活相机
    public float giveDamage = 10f; // 枪械造成的伤害值：命中目标时每次扣除的生命值
    public float shootingRange = 100f; // 枪械射击范围：射线检测的最大距离（单位：米）

    private void Start()
    {
        ResolveCam(); // 启动时自动解析一次相机，避免第一枪时cam为空
    }

    // 修复：自动解析当前激活的相机（AimCam或ThirdPersonCam谁激活用谁），未手动赋值时兜底；已手动赋值则不覆盖
    private void ResolveCam()
    {
        if (cam != null)
        {
            return;
        }
        foreach (Camera c in Camera.allCameras) // 只返回场景中已启用且在激活物体上的相机
        {
            if (c != null && c.gameObject.activeInHierarchy)
            {
                cam = c;
                break;
            }
        }
    }

    private void Update()
    {
        // 检测是否按下开火键：Fire1对应Input Manager中的Fire1轴（默认绑定鼠标左键）
        if (Input.GetButtonDown("Fire1")) // GetButtonDown表示"按下瞬间"触发一次，避免按住时连续开火
        {
            Shoot(); // 调用Shoot方法执行一次射击
        }
    }

    void Shoot() // 射击方法：从相机位置沿相机正前方发射射线，检测是否命中物体并造成伤害
    {
        // 防御：每次射击前先尝试自动解析相机（防止相机切换后引用失效），解析不到则直接返回，杜绝UnassignedReferenceException
        ResolveCam();
        if (cam == null)
        {
            return;
        }

        RaycastHit hitInfo; // 射线检测信息：用于接收被射线命中的物体的详细信息

        // 执行射线检测：从相机位置沿相机正前方发射射线，检测距离为shootingRange；命中物体时hitInfo填充信息并返回true
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, shootingRange))
        {
            Debug.Log(hitInfo.transform.name); // 在Console窗口输出被命中物体的名称，便于验证射击是否命中

            Objects objects = hitInfo.transform.GetComponent<Objects>(); // 获取被命中物体上的Objects组件（实现扣血和销毁）

            if (objects != null) // 命中物带Objects组件时才扣血（如敌人/目标物），普通地面墙体等不会误伤
            {
                objects.objectHitDamage(giveDamage); // 调用objectHitDamage方法，扣除giveDamage点生命值
            }
        }
    }
}




// using UnityEngine;
// using System.Collections.Generic; // 导入System.Collections.Generic命名空间（本脚本当前未直接使用，保留原样）
// using System.Collections; // 导入System.Collections命名空间（本脚本当前未直接使用，保留原样）

// // 步枪脚本：挂在玩家枪械物体上，负责监听开火输入、发射射线检测命中的目标
// public class Rifle : MonoBehaviour
// {
//     [Header("Rifle")] // 在Inspector窗口中创建"Rifle"分组标题，提高可读性
//     public Camera cam; // 相机引用：用于获取射击起点（相机位置）和射击方向（相机前方），即"从屏幕中心瞄准"
//     public float giveDamage = 10f; // 枪械造成的伤害值：命中目标时实际扣血量（本脚本当前只打印目标名，伤害值供后续伤害逻辑使用）
//     public float shootingRange = 100f; // 枪械射击范围：射线检测的最大距离（单位：米），超过该距离的目标不会被命中

//     private void Update() // 每帧调用：负责监听玩家的开火输入
//     {
//         // 检测是否按下开火键：Fire1对应Input Manager中的Fire1轴（默认绑定鼠标左键）
//         if (Input.GetButtonDown("Fire1")) // GetButtonDown表示"按下瞬间"触发一次，避免按住时连续开火
//         {
//             Shoot(); // 调用Shoot方法执行一次射击
//         }
//     }

//     void Shoot() // 射击方法：从相机位置沿相机正前方发射一条射线，检测是否命中物体
//     {
//         RaycastHit hitInfo; // 声明射线检测结果变量：用于接收被射线命中的物体的详细信息（碰撞点、碰撞体、物体名等）

//         // 执行射线检测：从cam.position（相机位置）沿cam.forward（相机正前方）发射射线，检测距离为shootingRange
//         // 若命中任意带碰撞体的物体，hitInfo会填充该物体的信息，方法返回true并进入if代码块
//         if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, shootingRange))
//         {
//             Debug.Log(hitInfo.transform.name); // 在Console窗口输出被命中物体的名称，用于验证射击是否成功命中
//         }
//     }
// }
