using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class PlayerAI : MonoBehaviour
{
    [Header("Player Health and Damagre")] // 玩家生命值和伤害值
    private float PlayerHealth = 200f; // 玩家生命值
    private float presentHealth; // 玩家当前生命值
    public float giveDamage = 10f; // 玩家造成的伤害值
    public float PlayerSpeed; // 玩家移动速度

    [Header("Player Things")] // 玩家其他属性
    public NavMeshAgent PlayerAgent; // 玩家导航代理组件Agent
    public Transform LookPoint; // 玩家观察点变换组件
    public GameObject ShootingRaycastArea; // 玩家射击射线检测区域
    public Transform enemyBody; // 敌人主体变换组件
    public LayerMask enemyLayer; // 敌人所在的图层
    public Transform Spawn; // 玩家重生点变换组件 
    public Transform PlayerCharacter; // 玩家角色变换组件

    [Header("Player Shooting Var")] // 玩家射击变量
    public float timebtwShoot; // 玩家射击间隔时间
    bool previouslyShoot; // 玩家是否已经射击过

    [Header("Player Animations and Spark effect")] // 玩家动画和火花效果
    public Animator anim; // 玩家动画组件

    [Header("Player States")] // 玩家状态
    public float visionRadius; // 玩家视野范围半径
    public float shootingRadius; // 玩家射击范围
    public bool enemyInvisionRadius; // 敌人是否在视野范围内
    public bool enemyInShootingRadius; // 敌人是否在射击范围内
    // public bool isPlayer = false; // 玩家是否存在
    

    private void Awake()
    {
        PlayerAgent = GetComponent<NavMeshAgent>(); // 获取玩家导航代理组件
        presentHealth = PlayerHealth; // 玩家当前生命值为玩家生命值
    }

    public void Update()
    {
        enemyInvisionRadius = Physics.CheckSphere(transform.position, visionRadius, enemyLayer); // 检测敌人是否在视野范围内
        enemyInShootingRadius = Physics.CheckSphere(transform.position, shootingRadius, enemyLayer); // 检测敌人是否在射击范围内

        if(enemyInvisionRadius && !enemyInShootingRadius) PursueEnemy(); // 如果玩家在视野范围内但不在射击范围内，追踪玩家
        if(enemyInvisionRadius && enemyInShootingRadius) ShootEnemy(); // 如果玩家在视野范围内且在射击范围内，射击玩家
    }

    private void PursueEnemy() // 玩家追踪敌人方法
    {
        if(PlayerAgent.SetDestination(enemyBody.position)) // 如果玩家导航代理组件设置目标位置成功
        {
            anim.SetBool("Running", true); // 玩家动画组件设置跑步状态为true
            anim.SetBool("Shooting", false); // 玩家动画组件设置射击状态为false
        }
        else
        {
            anim.SetBool("Running", false); // 玩家动画组件设置跑步状态为false
            anim.SetBool("Shooting", false); // 玩家动画组件设置射击状态为false
        }
    }

    private void ShootEnemy() // 玩家射击敌人方法

    {
        PlayerAgent.SetDestination(transform.position); // 玩家导航代理组件设置目标位置为玩家当前位置
        transform.LookAt(LookPoint); // 玩家变换组件设置观察点变换组件为目标位置

        if (!previouslyShoot) // 如果玩家没有射击过
        {
            RaycastHit hit; // 创建一个射线碰撞检测对象

            if(Physics.Raycast(ShootingRaycastArea.transform.position, ShootingRaycastArea.transform.forward, out hit, shootingRadius)) // 如果玩家射击射线检测区域发射射线并检测到碰撞
            {
                Debug.Log("Shooting" + hit.transform.name); // 打印射击目标名称
                // PlayerScript playerBody = hit.transform.GetComponent<PlayerScript>(); // 获取射线碰撞对象的脚本组件PlayerScript
                // if(playerBody != null)
                // {
                //     playerBody.playerHitDamage(giveDamage)
                // }
                
                Enemy enemy = hit.transform.GetComponent<Enemy>(); // 获取射线碰撞对象的组件Enemy
                
                if(enemy != null) // 如果射线碰撞对象的组件Enemy不为空
                {
                   enemy.enemyHitDamage(giveDamage); // 敌人受到伤害方法，传入玩家造成的伤害值
                }
                
            }

            anim.SetBool("Running", false); // 玩家动画组件设置跑步状态为false
            anim.SetBool("Shooting", true); // 玩家动画组件设置射击状态为true
        }

        previouslyShoot = true; // 玩家射击状态为true
        Invoke(nameof(ActiveShooting), timebtwShoot); // 调用玩家射击方法，传入玩家射击间隔时间
    }

    private void ActiveShooting() // 玩家射击方法
    {
        previouslyShoot = false; // 玩家射击状态为false
    }

    public void PlayerAIHitDamage(float takeDamage) // 玩家受到伤害方法
    {
        presentHealth -= takeDamage; // 玩家当前生命值减去玩家受到的伤害值

        if(presentHealth <= 0) // 如果玩家当前生命值小于等于0
        {
            StartCoroutine(Respawn()); // 调用玩家重生方法
        }
    }

    IEnumerator Respawn() // 玩家重生方法
    {
        PlayerAgent.SetDestination(transform.position); // 玩家导航代理组件设置目标位置为玩家当前位置
        PlayerSpeed = 0f; // 玩家速度为0
        shootingRadius = 0f; // 玩家射击范围为0
        visionRadius = 0f; // 玩家视野范围半径为0
        enemyInvisionRadius = false; // 敌人是否在视野范围内为false
        enemyInShootingRadius = false; // 敌人是否在射击范围内为false
        anim.SetBool("Die", true); // 玩家动画组件设置死亡状态为true
        anim.SetBool("Running", false); // 玩家动画组件设置跑步状态为false
        anim.SetBool("Shooting", false); // 玩家动画组件设置射击状态为false

        Debug.Log("Dead"); // 打印死亡信息

        yield return new WaitForSeconds(5f); // 等待5秒

        Debug.Log("Spawn"); // 打印重生信息

        presentHealth = 120f; // 玩家当前生命值为120
        PlayerSpeed = 1f; // 玩家速度为1
        shootingRadius = 10f; // 玩家射击范围为10
        visionRadius = 100f; // 玩家视野范围半径为100
        enemyInvisionRadius = true;  // 敌人是否在视野范围内为true
        enemyInShootingRadius = false; // 敌人是否在射击范围内为false

        anim.SetBool("Die", false); // 玩家动画组件设置死亡状态为false
        anim.SetBool("Running", true); // 玩家动画组件设置跑步状态为true

        PlayerCharacter.transform.position = Spawn.transform.position; // 玩家变换组件设置位置为重生点位置
        PursueEnemy(); // 调用玩家追踪敌人方法

    }
}
