using System.Collections;
using System.Collections.Generic;
// using System.Numerics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    public float runSpeed = 2.0f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Flip()
    {
        bool playerHasXSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        if (playerHasXSpeed){
            if(rb.velocity.x > 0.1f){
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }else if(rb.velocity.x < -0.1f){
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        Flip();
    }

    void Run()
    {
        //检测玩家的竖直和水平输入，然后将其转换为向量，然后将其传递给刚体的速度
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

        //传递速度
        //transform.position = transform.position + movement * Time.deltaTime * runSpeed;
        rb.velocity = movement * runSpeed;





        // float moveDir = Input.GetAxis("Horizontal");
        // float moveVertical = Input.GetAxis("Vertical");

        // Vector2 playerVertical = new Vector2(rb.velocity.x,moveVertical * runSpeed);
        // Vector2 playerVel = new Vector2(moveDir * runSpeed, rb.velocity.y);
        // rb.velocity = playerVel;
        bool playerHasXSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        animator.SetBool("Running", playerHasXSpeed);
    }
}
