using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        CoinsCounter.Instance.NumberOfCoins++;
        Destroy(gameObject);
    }
}
