using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingImageObj : MonoBehaviour
{
    [SerializeField]
    Renderer renderer;

    float time = 2;
    float count = 0;
    bool isTracking = false;
    public void SetTexture(Texture texture)
    {
        renderer.material.SetTexture("_Albedo", texture);
    }
    public void SetColor(Color color)
    {
        renderer.material.SetColor("_Color", color);
    }
    private void Update()
    {
        if (!isTracking)
        {
            count += Time.deltaTime;
            if (count > time)
            {
                gameObject.SetActive(false);
            }
        }
    }
    public void SetTracking(bool isTracking)
    {
        this.isTracking = isTracking;
        if (isTracking)
        {
            gameObject.SetActive(isTracking);
            count = 0;
        }
    }
}
