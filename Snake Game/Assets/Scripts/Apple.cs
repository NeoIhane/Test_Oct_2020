using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer sprite;

    int totalApple = 0;
    int maxApple = 0;
    Vector3 startApplePos;

    void Start()
    {
        InitApple();
    }

    public void InitApple()
    {
        startApplePos = transform.position;
    }
    
    public void PlaceAppleAtStartPoint()
    {
        transform.position = startApplePos;
    }
    public void Eat()
    {
        sprite.gameObject.SetActive(false);
    }
    public void Place(Vector3 position)
    {
        transform.position = position;
        sprite.gameObject.SetActive(true);
    }
}
