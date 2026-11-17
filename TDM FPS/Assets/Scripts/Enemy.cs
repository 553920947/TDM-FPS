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

    [Header("Enemy Shooting Var")]
    public float timebtwShoot; 
    bool previouslyShoot; 

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
        }
    }

    private void ShootPlayer()
    {
        enemyAgent.SetDestination(transform.position); // 停止移动，保持当前位置
        transform.LookAt(LookPoint); // 让敌人朝向观察点

        if (!previouslyShoot)
        {
            RaycastHit hit;

            if(Physics.Raycast(ShootingRaycastArea.transform.position, ShootingRaycastArea.transform.forward, out hit, shootingRadius))
            {
                Debug.Log("Shooting" + hit.transform.name);

                PlayerScript playerBody = hit.transform.GetComponent<PlayerScript>(); // 获取玩家的脚本组件

                if(playerBody != null)
                {
                    playerBody.playerHitDamage(giveDamage); // 调用玩家的受伤方法，传入敌人造成的伤害值
                }
            }
        }

        previouslyShoot = true;
        Invoke(nameof(ActiveShooting), timebtwShoot);
    }

    private void ActiveShooting()
    {
        previouslyShoot = false; 
    }

    public void enemyHitDamage(float takeDamage) // 敌人受到伤害的方法
    {
        presentHealth -= takeDamage; // 敌人受到伤害，减少当前生命值

        if(presentHealth <= 0)
        {
            Respawn(); // 如果当前生命值小于等于0，调用重生方法
        }
    }

    void Respawn()
    {
        enemyAgent.SetDestination(transform.position); // 停止移动，保持当前位置
        enemySpeed = 0f; // 敌人速度为0，无法移动
        shootingRadius = 0f; // 敌人射击范围为0，无法射击
        visionRadius = 0f; // 敌人视野范围半径为0，无法检测玩家
        playerInvisionRadius = false; // 玩家不在敌人的视野范围内
        playerInshootingRadius = false; // 玩家不在敌人的射击范围内
    }
}
