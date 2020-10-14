using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeNode : MonoBehaviour
{
    public Vector3 currentPosition { get; private set; }
    public Vector3 nextPosition { get; private set; }
    public Vector3 direction { get; private set; }

    public void Set(Vector3 position, Vector3 direction, float blockSize)
    {
        this.direction = direction;
        transform.position = currentPosition = position;
        nextPosition = position + direction * blockSize;
    }
    /// <param name="value">0.0f-1.0f</param>
    public void MoveNext(float value)
    {
        transform.position = Vector3.Lerp(currentPosition, nextPosition, value);
    }

    public void SetToNextNode(SnakeNode snakeNode, float blockSize)
    {
        Set(snakeNode.currentPosition, snakeNode.direction, blockSize);
    }
    public virtual void SetSize(float size, bool relateDirection=false) { }
}

