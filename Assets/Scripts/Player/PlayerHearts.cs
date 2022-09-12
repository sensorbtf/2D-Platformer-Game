using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHearts : MonoBehaviour
{
    [SerializeField] private int  numberOfHearts = 3;
    [SerializeField] private Image[]  hearts;
    [SerializeField]  private Sprite  fullHeart;
    [SerializeField] private Sprite  brokenHeart;

    public int NumberOfHearts
    {
        get { return  numberOfHearts; }
        set {  numberOfHearts = value < 0 ?  numberOfHearts = 0 :  numberOfHearts = value; }
    }

    private void Update()
    {

        if (PlayerConfig.Instance.Health > NumberOfHearts)
            PlayerConfig.Instance.Health = NumberOfHearts;

        for (int i = 0; i <  hearts.Length; i++)
        {
            if (i < NumberOfHearts)
                 hearts[i].enabled = true;
            else
                 hearts[i].enabled = false;

            if (i < PlayerConfig.Instance.Health)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = brokenHeart;
        }
    }
}
