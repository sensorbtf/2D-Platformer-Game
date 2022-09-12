using UnityEngine;
using Zenject;

public class ProjectileInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<ProjectileSpawner>().AsSingle();
        Container.Bind<ShootingEnemy>().AsSingle();
        Container.BindFactory<Projectile, Projectile.Factory>();
    }
}