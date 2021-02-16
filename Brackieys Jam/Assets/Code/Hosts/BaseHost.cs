using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHost : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] protected Rigidbody2D Rigidbody;
    [SerializeField] protected Animator animator;

    [SerializeField] protected BaseParsite Parasite;
    [SerializeField] protected Transform ParasiteOrigin;

    [Header("Data")]
    [SerializeField] protected int Health = 10;

    [SerializeField] protected float baseForwardSpeed;
    [SerializeField] protected float baseStrafeSpeed;

    [SerializeField] protected float BounceBackForce;


    protected Vector2 inputValue;
    protected Vector2 direction;

    public virtual void InitializeHost()
    {
        // Put health and stuff here
    }

    public virtual void ChangeParasite(BaseParsite parasite)
    {
        Parasite = parasite;
        Parasite.transform.parent = transform;
        Parasite.transform.position = ParasiteOrigin.position;
        Parasite.transform.rotation = Quaternion.identity;
        Parasite.SetupParasite();
    }

    protected virtual void LookAtMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        direction = new Vector2(mousePos.x - Rigidbody.transform.position.x, mousePos.y - Rigidbody.transform.position.y);
        Rigidbody.transform.up = direction.normalized;
    }
}
