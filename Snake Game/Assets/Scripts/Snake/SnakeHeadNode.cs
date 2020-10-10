using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHeadNode : SnakeNode
{
    public enum ActionState { Normal, Eating, Die }

    [SerializeField]
    ActionState actionState;

    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Animator animator;

    public Action onEatSelf;
    public Action onEatApple;
    public Action onHitWall;

    public void SetDirection(Vector3 direction, float blockSize)
    {
        Set(transform.position, direction, blockSize);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="SnakeBody")
        {
            var bodyNode = collision.GetComponent<SnakeBodyNode>();
            if (bodyNode != null)
            {
                if ((bodyNode.currentPosition- currentPosition) == direction)
                {
                    if (onEatSelf != null)
                        onEatSelf();
                    //Debug.Log("Hit!!");
                }
            }
        }
        else if (collision.tag == "Apple")
        {
            if(onEatApple != null)
                onEatApple();
        }else if(collision.tag=="Wall")
        {
            if (onHitWall != null)
                onHitWall();
        }
        //Debug.Log("Hit: " + collision.tag + " " + collision.name);
    }
    public void SetToEating()
    {
        actionState = ActionState.Eating;
        animator.SetTrigger("Eat");
    }
    public void SetToNormal()
    {
        actionState = ActionState.Normal;
        animator.SetTrigger("Idle");
    }
    public void SetToStun()
    {
        actionState = ActionState.Die;
        animator.SetTrigger("Stun");
    }
    private void Update()
    {
        switch(actionState)
        {
            case ActionState.Normal:
                break;
            case ActionState.Eating:
                break;
            case ActionState.Die:
                break;
        }
    }
}
