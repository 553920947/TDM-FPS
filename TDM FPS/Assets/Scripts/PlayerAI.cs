using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class PlayerAI : MonoBehaviour
{
    [Header("Player Health and Damagre")] // 玩家生命值和伤害值
    private float playerHealth = 200f; // 玩家生命值
    private float presentHealth; // 玩家当前生命值
    public float giveDamage = 10f; // 玩家造成的伤害值
    public float enemySpeed; // 玩家移动速度

    [Header("Player Things")] // 玩家其他属性
    public NavMeshAgent enemyAgent; // 玩家导航代理组件Agent
    public Transform LookPoint; // 玩家观察点变换组件
    public GameObject ShootingRaycastArea; // 玩家射击射线检测区域
    public Transform playerBody; // 玩家主体变换组件
    public LayerMask PlayerLayer; // 玩家所在的图层
    public Transform Spawn; // 玩家重生点变换组件 
    public Transform EnemyCharacter; // 玩家角色变换组件

    [Header("Enemy Shooting Var")] // 玩家射击变量
    public float timebtwsHOOT; // 玩家射击间隔时间
    bool previouslyShoot; // 玩家是否已经射击过

    [Header("Enemy Animations and Spark effect")] // 玩家动画和火花效果
    public Animator anim; // 玩家动画组件

    [Header("Enemy States")] // 玩家状态
    public float visionRadius; // 玩家视野范围半径
    public float shootingRadius; // 玩家射击范围
    public bool playerInvisionRadius; // 玩家是否在视野范围内
    public bool playerInshootingRadius; // 玩家是否在射击范围内
    public bool isPlayer = false; // 玩家是否存在

    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>(); // 获取玩家导航代理组件
        presentHealth = enemySpeed; // 玩家当前生命值为玩家移动速度
    }

    public void Update()
    {
        playerInvisionRadius = Physics.CheckSphere(transform.position, visionRadius, PlayerLayer); // 检测玩家是否在视野范围内
        playerInshootingRadius = Physics.CheckSphere(transform.position, shootingRadius, PlayerLayer); // 检测玩家是否在射击范围内

        if(playerInvisionRadius && !playerInshootingRadius) Pursueplayer(); // 如果玩家在视野范围内但不在射击范围内，追踪玩家
        if(playerInvisionRadius && playerInshootingRadius) ShootPlayer(); // 如果玩家在视野范围内且在射击范围内，射击玩家
    }

    private void Pursueplayer()
    {
        if(enemyAgent.SetDestination(playerBody.position)) // 如果玩家导航代理组件设置目标位置成功
        {
            anim.SetBool("Running", true); // 玩家动画组件设置跑步状态为true
        }
    }

    private void ShootPlayer()

    {
        
    }
}
