using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Health and Damage")] // 敌人的生命值和伤害值
    private float enemyHealth = 120f; // 敌人的生命值
    private float presentHealth; // 敌人的当前生命值
    public float giveDamage = 5f; // 敌人造成的伤害值
    public float enemySpeed; // 敌人的移动速度

    [Header("Enemy Things")]
    public NavMeshAgent enemyAgent; // 敌人的导航代理组件
    public Transform LookPoint; // 敌人的观察点变换组件
    public GameObject ShootingRaycastArea; // 敌人的射击射线检测区域
    public Transform playerBody; // 玩家的主体变换组件 
    public LayerMask PlayerLayer; // 玩家所在的图层
    public Transform Spawn; // 敌人的重生点变换组件
    public Transform EnemyCharacter; // 敌人的角色变换组件

    [Header("Enemy Shooting Var")]
    public float timebtwShoot; // 敌人的射击时间间隔
    bool previouslyShoot; // 敌人是否已射击

    [Header("Enemy Animations and Spark effect")]
    public Animator anim; // 敌人的动画组件

    [Header("Enemy States")]
    public float visionRadius; // 敌人的视野范围半径
    public float shootingRadius; // 敌人的射击范围
    public bool playerInvisionRadius; // 玩家是否在敌人的视野范围内
    public bool playerInshootingRadius; // 玩家是否在敌人的射击范围内
    public bool isPlayer = false; // 玩家是否存在

    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>(); // 获取敌人的导航代理组件
        presentHealth = enemyHealth; // 敌人的当前生命值为敌人的生命值    
    }

    private void Update()
    {
        playerInvisionRadius = Physics.CheckSphere(transform.position, visionRadius, PlayerLayer); // 检测玩家是否在敌人的视野范围内
        playerInshootingRadius = Physics.CheckSphere(transform.position, shootingRadius, PlayerLayer); // 检测玩家是否在敌人的射击范围内

        if(playerInvisionRadius && !playerInshootingRadius) Pursueplayer(); // 如果玩家在视野范围内但不在射击范围内，追踪玩家
        if(playerInvisionRadius && playerInshootingRadius) ShootPlayer();
        
    }

    private void Pursueplayer() // 追踪玩家
    {
        if (enemyAgent.SetDestination(playerBody.position)) // 设置导航代理的目标为玩家位置
        {
            // animations // 敌人移动到玩家位置时，播放动画
            anim.SetBool("Running", true); // 播放敌人奔跑动画
            anim.SetBool("Shooting", false); // 停止播放敌人射击动画
        }
        else
        {
            anim.SetBool("Running", false); // 停止播放敌人奔跑动画
            anim.SetBool("Shooting", false); // 停止播放敌人射击动画
        }
    }

    private void ShootPlayer() // 射击玩家
    {
        enemyAgent.SetDestination(transform.position); // 停止移动，保持当前位置
        transform.LookAt(LookPoint); // 让敌人朝向观察点

        if (!previouslyShoot) // 如果敌人射击状态为false
        {
            RaycastHit hit; // 射线检测结果

            if(Physics.Raycast(ShootingRaycastArea.transform.position, ShootingRaycastArea.transform.forward, out hit, shootingRadius)) // 如果射线检测到玩家
            {
                Debug.Log("Shooting" + hit.transform.name); // 打印日志，通知敌人正在射击玩家

                if(isPlayer == true) // 如果玩家存在
                {
                    PlayerScript playerBody = hit.transform.GetComponent<PlayerScript>(); // 获取玩家脚本组件
                    if(playerBody != null) // 如果玩家脚本组件存在
                    {
                        playerBody.playerHitDamage(giveDamage);
                    }
                }
                else
                {
                    PlayerAI playerBody = hit.transform.GetComponent<PlayerAI>(); // 获取玩家AI脚本组件
                    if(playerBody != null) // 如果玩家AI脚本组件存在
                    {
                        playerBody.PlayerAIHitDamage(giveDamage); // 玩家AI受到伤害
                    }
                }

            }

            anim.SetBool("Running", false);
            anim.SetBool("Shooting", true);
        }

        previouslyShoot = true;
        Invoke(nameof(ActiveShooting), timebtwShoot); // 调用ActiveShooting方法，传入敌人射击时间间隔
    }

    private void ActiveShooting() // 敌人射击方法
    {
        previouslyShoot = false;  // 敌人射击状态为false，准备下一次射击
    }

    public void enemyHitDamage(float takeDamage) // 敌人受到伤害的方法
    {
        presentHealth -= takeDamage; // 敌人受到伤害，减少当前生命值

        if(presentHealth <= 0) // 如果敌人当前生命值小于等于0
        {
            StartCoroutine(Respawn()); // 调用重生方法，传入敌人死亡的等待时间
        }
    }

    IEnumerator Respawn() // 敌人重生方法
    {
        enemyAgent.SetDestination(transform.position); // 停止移动，保持当前位置
        enemySpeed = 0f; // 敌人速度为0，无法移动
        shootingRadius = 0f; // 敌人射击范围为0，无法射击
        visionRadius = 0f; // 敌人视野范围半径为0，无法检测玩家
        playerInvisionRadius = false; // 玩家不在敌人的视野范围内
        playerInshootingRadius = false; // 玩家不在敌人的射击范围内
        anim.SetBool("Die", true); // 播放敌人死亡动画
        anim.SetBool("Running", false); // 停止播放敌人奔跑动画
        anim.SetBool("Shooting", false); // 停止播放敌人射击动画

        // animations // 播放敌人死亡动画

        Debug.Log("已死亡！"); // 打印日志，通知敌人已死亡
        
        yield return new WaitForSeconds(5f); // 等待5秒后重生
        // 重生逻辑

        Debug.Log("已重生！"); // 打印日志，通知敌人已重生

        presentHealth = 120f; // 重生后恢复敌人的生命值
        enemySpeed = 1f; // 重生后恢复敌人的移动速度
        shootingRadius = 10f; // 重生后恢复敌人的射击范围
        visionRadius = 100f; // 重生后恢复敌人的视野范围半径
        playerInvisionRadius = true; // 玩家在敌人的视野范围内
        playerInshootingRadius = false; // 玩家不在敌人的射击范围内

        // animations // 播放敌人重生动画
        anim.SetBool("Die", false); 
        anim.SetBool("Running", true);

        // spawn point // 设置敌人的重生点位置
        EnemyCharacter.position = Spawn.position; // 将敌人的位置设置为重生点的位置
        Pursueplayer(); // 追踪玩家
    }
}
