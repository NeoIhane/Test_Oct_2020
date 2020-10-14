using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Popup : MonoSingleton<Popup>
{
    [SerializeField]
    Animator animator;
    [SerializeField]
    Text scoreTitleText;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Button button;

    Action onPlayAgain;

    private void Start()
    {
        button.onClick.AddListener(PlayAgain);
    }

    public void Init(Action playAgain_callback)
    {
        onPlayAgain = playAgain_callback;
    }
    public void Show(int score, bool isWin = false)
    {
        animator.SetTrigger("Show");
        if (isWin) scoreTitleText.text = string.Format("Congratulation! :)", score.ToString());
        else
            scoreTitleText.text = "Your score";
        scoreText.text = score.ToString();
    }
    public void Hide()
    {
        animator.SetTrigger("Hide");
    } 
    void PlayAgain()
    {
        if (onPlayAgain != null) onPlayAgain();
    }
    
}
