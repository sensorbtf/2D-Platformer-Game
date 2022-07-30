using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsCounter : MonoBehaviour, ICollectibles
{
    public static CoinsCounter Instance { get; private set; }

    [SerializeField] private TMPro.TextMeshProUGUI coinsCounter;

    private int numberOfCoins = 0;

    public int NumberOfCoins
    {
        get { return numberOfCoins; }
        set { numberOfCoins = value < 0 ? numberOfCoins = 0 : numberOfCoins = value; }
    }

    private void FixedUpdate()
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
