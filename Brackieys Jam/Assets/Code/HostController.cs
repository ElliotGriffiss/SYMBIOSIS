using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class HostController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Rigidbody2D Rigidbody;

    [SerializeField] private BaseParsite Parsite;

    [Header("Data")]
    [SerializeField] protected int Health = 10;

    [SerializeField] protected float baseForwardSpeed;
    [SerializeField] protected float baseStrafeSpeed;

    [SerializeField] protected float BounceBackForce;


    private Vector2 inputValue;
    private Vector2 direction;

    protected virtual void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Parsite.FireBullet(direction);
        }

        inputValue.x = Input.GetAxisRaw("Horizontal"); //Setting the x and y values of the "movement" var based on what keys are down
        inputValue.y = Input.GetAxisRaw("Vertical"); //^^

        LookAtMouse();
    }

    protected void OnCollisionEnter2D(Collision2D collision)
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

    protected virtual void FixedUpdate()  
    {
        Vector2 force = Vector2.zero;

        if (inputValue.y > 0f)
        {
            force += direction.normalized * inputValue.y * baseForwardSpeed;
        }

        Vector2 rightDirection = Rigidbody.transform.right.normalized;

        if (inputValue.x > 0)
        {
            force += (rightDirection.normalized * inputValue.x * baseStrafeSpeed);
        }
        
        if (inputValue.x < 0)
        {
             force += (rightDirection.normalized * inputValue.x * baseStrafeSpeed);
        }

        Rigidbody.AddForce(force);
    }

    protected virtual void LookAtMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        direction = new Vector2(mousePos.x - Rigidbody.transform.position.x, mousePos.y - Rigidbody.transform.position.y);
        Rigidbody.transform.up = direction.normalized;
    }
}
