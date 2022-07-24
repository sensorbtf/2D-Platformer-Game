using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsCounter : MonoBehaviour
{
    public static CoinsCounter Instance { get; private set; }
    [SerializeField]
    private TMPro.TextMeshProUGUI coinsCounter;

    private int _numberOfCoins = 0;

    public int NumberOfCoins
    {
        get { return _numberOfCoins; }
        set { _numberOfCoins = value < 0 ? _numberOfCoins = 0 : _numberOfCoins = value; }
    }

    void FixedUpdate()
    {
        coinsCounter.text = NumberOfCoins.ToString();
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
}
