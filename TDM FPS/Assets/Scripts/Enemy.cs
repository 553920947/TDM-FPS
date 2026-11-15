using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Health and Damage")]
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
            }
        }

        previouslyShoot = true;
        Invoke(nameof(ActiveShooting), timebtwShoot);
    }

    private void ActiveShooting()
    {
        previouslyShoot = false; 
    }
}
