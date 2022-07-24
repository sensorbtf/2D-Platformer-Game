using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHearts : MonoBehaviour
{
    [SerializeField]
    private int _numberOfHearts = 3;
    [SerializeField]
    private Image[] _hearts;
    [SerializeField]
    private Sprite _fullHeart;
    [SerializeField]
    private Sprite _brokenHeart;

    public int NumberOfHearts
    {
        get { return _numberOfHearts; }
        set { _numberOfHearts = value < 0 ? _numberOfHearts = 0 : _numberOfHearts = value; }
    }

    private void Update()
    {

        if (Player.Instance.Health > NumberOfHearts)
        {
            Player.Instance.Health = NumberOfHearts;
        }

        for (int i = 0; i < _hearts.Length; i++)
        {
            if (i < NumberOfHearts)
            {
                _hearts[i].enabled = true;
            }
            else
            {
                _hearts[i].enabled = false;
            }

            if (i < Player.Instance.Health)
            {
                _hearts[i].sprite = _fullHeart;
            }
            else
            {
                _hearts[i].sprite = _brokenHeart;
            }

        }
    }
}
