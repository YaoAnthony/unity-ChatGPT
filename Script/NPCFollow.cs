using UnityEngine;

public class NPCFollow : MonoBehaviour
{
    public bool isFollow;
    public Transform player; // 玩家的Transform组件
    private Animator animator; // NPC的Animator组件
    public Rigidbody2D rb; // NPC的Rigidbody2D组件
    public float moveSpeed = 5f; // NPC的移动速度
    public float stoppingDistance = 1f; // NPC与玩家之间的停止距离

    private Vector2 movement; // NPC的移动方向

    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>(); // 确保获取了Rigidbody2D组件
        animator = GetComponent<Animator>();
        isFollow = false;
    }

    void Update()
    {
        if (isFollow)
        {
            FollowPlayer();
        }
        
    }

    void FollowPlayer(){
        // 计算从NPC到玩家的向量
        Vector2 direction = player.position - transform.position;
        float distance = direction.magnitude;

        // 如果NPC与玩家的距离大于stoppingDistance，则计算移动方向和速度
        if (distance > stoppingDistance)
        {
            movement = direction.normalized;
        }
        else
        {
            // 如果NPC与玩家足够近，则停止移动
            movement = Vector2.zero;
        }

        bool ObjectHasXSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        animator.SetBool("isRunning", ObjectHasXSpeed);

        Flip();
    }

    void Flip()
    {
        bool playerHasXSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        if (playerHasXSpeed)
        {
            if(rb.velocity.x > 0.1f)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);

            }else if(rb.velocity.x < -0.1f)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }
    void FixedUpdate()
    {
        // 根据玩家的位置和NPC的移动速度调整NPC的速度
        if (movement != Vector2.zero)
        {
            rb.velocity = movement * moveSpeed;
        }
        else
        {
            // 当不需要移动时，将速度设置为0
            rb.velocity = Vector2.zero;
        }
    }
}
