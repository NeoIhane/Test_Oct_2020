using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Photo : MonoBehaviour
{
    [SerializeField]
    RawImage rawImage;
    [SerializeField]
    Text nameText;
    public void Set(Texture2D texture, string name)
    {
        SetImage(texture);
        SetName(name);
    }
    public void SetImage(Texture2D texture)
    {
        rawImage.texture = texture;
    }
    public void SetName(string name)
    {
        nameText.text = name;
    }
}
