using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    float time;
    float count;
    bool isMove = false;
    Transform target = null;
    Vector3 tmpPosition;
    [SerializeField]
    GameObject sphere;
    public Color color { get; private set; }
    void Update()
    {
        if (target != null)
        {
            if (isMove)
            {
                count += Time.deltaTime;
                if (count >= time)
                {
                    isMove = false;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, target.position, count / time);
                }
            }
            else
            {
                transform.position = target.position;
            }
        }
        else
        {
            Despawn();
        }
    }
    public void SetActive(bool enable)
    {
        gameObject.SetActive(enable);
    }
    
    public Transform GetTarget()
    {
        return target;
    }
    public void ChangeToNewTarget(Transform target, float time)
    {
        tmpPosition = transform.position;
        this.target = target;
        this.time = time;
        count = 0;
        isMove = true;
    }
    public void Despawn()
    {
        SetActive(false);
    }
    public void Spawn(Color color, Transform target)
    {
        this.target = target;
        tmpPosition = transform.position;
        SetColor(color);
        count = 0;
        isMove = false;
    }
    void SetColor(Color color)
    {
        sphere.GetComponent<Renderer>().material.SetColor("_Color", color);
        this.color = color;
    }
}
