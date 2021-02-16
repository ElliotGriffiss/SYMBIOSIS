using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleController : BaseHost
{
    [SerializeField] protected float waitTime;
    [SerializeField] protected float moveTime;

    private bool canMove = true;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Parasite.ActivateParasite(direction);
        }
        if (canMove == true)
        {
            StartCoroutine(MoveCo(waitTime, moveTime));
        }

        LookAtMouse();
    }
    public override void HandleCollisonEnter(Collision2D collision)
    {
        if (collision.gameObject.tag != "Bullet")
        {
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = 0f;
            Rigidbody.AddForce((collision.transform.position + transform.position).normalized * BounceBackForce, ForceMode2D.Impulse);

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
        Rigidbody.AddForce(direction.normalized * baseForwardSpeed, ForceMode2D.Impulse);
        animator.SetBool("isMoving", true);
        yield return new WaitForSeconds(moveTime);
        Rigidbody.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        canMove = true;
    }
}
