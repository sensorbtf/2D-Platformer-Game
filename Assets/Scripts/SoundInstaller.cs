using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class SoundInstaller : MonoInstaller
{
    public AudioSource MusicSource;
    public AudioSource PlayerEffectsSource;
    public AudioSource PlayerWalkingSource;
    public AudioSource EnemyEffectsSource;
    public AudioSource EnviromentEffectSource;

    public override void InstallBindings()
    {
        Container.Bind<SoundManager>().AsSingle().WithArguments(MusicSource, PlayerEffectsSource, PlayerWalkingSource, EnemyEffectsSource, EnviromentEffectSource).NonLazy();
    }
}