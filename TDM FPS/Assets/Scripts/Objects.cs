using UnityEngine;

public class Objects : MonoBehaviour
{
    public float objectHealth = 100f; // 物体的初始生命值

    public void objectHitDamage(float amount)
    {
        objectHealth -= amount; // 扣除生命值
        
        if (objectHealth <= 0f)
        {
            Die(); // 如果生命值小于等于0，调用Die方法处理物体死亡
        }
    }

    void Die()
    {
        Destroy(gameObject); // 销毁物体
    }
}
