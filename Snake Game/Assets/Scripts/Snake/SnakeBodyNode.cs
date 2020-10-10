﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyNode : SnakeNode
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
    public override void SetSize(float size)
    {
        spriteRenderer.transform.localScale = new Vector3(1, 1, 1) + new Vector3(Mathf.Abs(direction.y) * size, Mathf.Abs(direction.x) * size, 1);
    }
}
