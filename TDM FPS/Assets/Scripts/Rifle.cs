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
    public float fireCharge = 15f; // 枪械开火冷却时间：开火后多少秒内不能再次开火（单位：秒）
    public PlayerScript player; // 玩家脚本引用：用于获取玩家的生命值和装填状态
    public Animator animator; // 动画控制器引用：用于控制枪械的开火和装填动画状态

    [Header("Rifle Ammunition and shooting")]
    private float nextTimeToShoot = 0f; // 下一次开火的时间点：用于记录上一次开火后多少秒内不能再次开火（单位：秒）
    private int maximumAmmunition = 20; // 最大弹药量：枪械一次性可装填的子弹数量
    private int mag = 15; // 当前弹药量：枪械当前已装填的子弹数量
    private int presentAmmunition; // 当前弹药量：枪械当前已装填的子弹数量（用于UI显示）
    public float reloadingTime = 1.3f; // 装填时间：枪械装填子弹所需的时间（单位：秒）
    private bool setReloading = false; // 是否正在装填：用于控制装填状态，防止在装填过程中再次开火

    private void Start()
    {
        ResolveCam(); // 启动时自动解析一次相机，避免第一枪时cam为空
    }
    private void Awake() // Awake方法：在脚本实例化时调用，用于初始化当前弹药量
    {
        presentAmmunition = maximumAmmunition; // 初始化当前弹药量为最大弹药量
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
        if (setReloading)
        return; // 如果正在装填，则不允许开火，直接返回

        if (presentAmmunition <= 0) // 如果当前弹药量为0，则开始装填
        {
            StartCoroutine(Reload()); // 调用Reload协程方法，开始装填
            return; // 装填过程中不允许开火，直接返回

        }

        if(Input.GetButton("Fire1") && Time.time >= nextTimeToShoot) // 检测是否按下开火键：Fire1对应Input Manager中的Fire1轴（默认绑定鼠标左键）；且当前时间点大于等于下一次开火时间点
        {
            animator.SetBool("Fire", true); // 设置动画状态为开火状态
            animator.SetBool("Idle", false); // 设置动画状态为非空闲状态

            nextTimeToShoot = Time.time + 1f/fireCharge; // 更新下一次开火时间点：当前时间点 + 开火冷却时间
            Shoot(); // 调用Shoot方法执行一次射击
        }
        else if (Input.GetButton("Fire1") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) // 检测是否按下开火键：Fire1对应Input Manager中的Fire1轴（默认绑定W或上箭头键）；且当前时间点大于等于下一次开火时间点
        {
            animator.SetBool("Idle", false); // 设置动画状态为非空闲状态
            animator.SetBool("FireWalk", true); // 设置动画状态为开火行走状态
        }
        else if (Input.GetButton("Fire1") && Input.GetButton("Fire2")) // 检测是否按下开火键：Fire1对应Input Manager中的Fire1轴（默认绑定鼠标左键）；且当前时间点大于等于下一次开火时间点
        {
            animator.SetBool("Idle", false);  // 设置动画状态为非空闲状态
            animator.SetBool("IdleAim", true); // 设置动画状态为瞄准状态
            animator.SetBool("FireWalk", true); // 设置动画状态为开火行走状态
            animator.SetBool("Walk", true); // 设置动画状态为行走状态
            animator.SetBool("Reloading", false); // 设置动画状态为非装填状态
        }
        else
        {
            animator.SetBool("Fire", false);  // 设置动画状态为非开火状态
            animator.SetBool("Idle", true); // 设置动画状态为空闲状态
            animator.SetBool("FireWalk", false); // 设置动画状态为非开火行走状态
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

        if(mag == 0) // 如果当前弹药量为0，则不允许开火
        {
            // show ammo out text
        }

        presentAmmunition--; // 扣除当前弹药量：每次开火消耗一发子弹

        if(presentAmmunition == 0) // 如果当前弹药量为0，则不允许开火
        {
            mag--; // 扣除当前弹药量：每次开火消耗一发子弹
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

    IEnumerator Reload() // 装填方法：在装填时间结束时恢复当前弹药量和玩家速度，并允许再次开火
    {
        player.playerSpeed = 0f; // 装填时玩家速度为0，无法移动
        player.playerSprint = 0f; // 装填时玩家冲刺为0，无法冲刺
        setReloading = true; // 设置装填状态为true，防止在装填过程中再次开火
        Debug.Log("Reloading..."); // 在Console窗口输出装填提示
        // animtion and audio  
        animator.SetBool("Reloading", true); // 设置动画状态为装填状态
        yield return new WaitForSeconds(reloadingTime); // 等待装填时间结束 
        // animation 
        animator.SetBool("Reloading", false); // 设置动画状态为非装填状态
        presentAmmunition = maximumAmmunition; // 装填完成后将当前弹药量恢复为最大弹药量
        player.playerSpeed = 1.9f; // 装填完成后恢复玩家速度
        player.playerSprint = 3f; // 装填完成后恢复玩家冲刺速度
        setReloading = false; // 装填完成后将装填状态设置为false，允许再次开火
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
