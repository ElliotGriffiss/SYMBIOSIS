using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [SerializeField] private BaseParsite Parsite;

    [Header("Data")]
    [SerializeField] protected int Health = 10;

    [SerializeField] protected float speed;

    [SerializeField] protected float waitTime;
    [SerializeField] protected float moveTime;

    [SerializeField] protected float BounceBackForce;

    private bool canMove = true;

    private Vector2 dir;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Parsite.ActivateParasite(dir);
        }
        if (canMove == true)
        {
            StartCoroutine(MoveCo(waitTime, moveTime));
        }
        LookAtMouse();
    }
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Bullet")
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;
            rb.AddForce((collision.transform.position + transform.position).normalized * BounceBackForce, ForceMode2D.Impulse);

            Health -= 2;
            Debug.Log("You got hit");

            if (Health < 1)
            {
                Debug.Log("You died!");
            }
        }
    }
    
    private IEnumerator MoveCo(float waitTime, float moveTime)
    {
        canMove = false;
        yield return new WaitForSeconds(waitTime);
        rb.AddForce(dir.normalized * speed, ForceMode2D.Impulse);
        animator.SetBool("isMoving", true);
        yield return new WaitForSeconds(moveTime);
        rb.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        canMove = true;
    }
    protected virtual void LookAtMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        dir = new Vector2(mousePos.x - rb.transform.position.x, mousePos.y - rb.transform.position.y);
        rb.transform.up = dir.normalized;
    }
}
