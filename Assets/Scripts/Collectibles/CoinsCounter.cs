using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CoinsCounter : ICollectibles, IFixedTickable
{
    [SerializeField] private TMPro.TextMeshProUGUI coinsCounter;

    private int _numberOfCoins = 0;
    public int NumberOfCoins
    {
        get { return _numberOfCoins; }
        set { _numberOfCoins = value < 0 ? _numberOfCoins = 0 : _numberOfCoins = value; }
    }
    public CoinsCounter(int coinCounter, TMPro.TextMeshProUGUI coinsCounterText)
    {
        _numberOfCoins = coinCounter;
        coinsCounter = coinsCounterText;
    }
    public void FixedTick()
    {
        coinsCounter.text = NumberOfCoins.ToString();
    }
}
