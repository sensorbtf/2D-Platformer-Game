using UnityEngine;
using Zenject;

public class CollectiblesInstaller : MonoInstaller
{
    public int NumberOfCoins;
    public TMPro.TextMeshProUGUI coinsCounterText;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<CoinsCounter>().AsSingle().WithArguments(NumberOfCoins, coinsCounterText).NonLazy();
    }
}