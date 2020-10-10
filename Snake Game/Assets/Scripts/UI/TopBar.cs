using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TopBar : MonoSingleton<TopBar>
{
    [SerializeField]
    Text appleText;
    [SerializeField]
    Text thophyText;

    public void SetNumApple(int total)
    {
        appleText.text = total.ToString();
    }
    public void SetNumThophy(int total)
    {
        thophyText.text = total.ToString();
    }
}
