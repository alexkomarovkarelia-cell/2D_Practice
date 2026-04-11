using UnityEngine;
using Zenject;

// Этот инсталлер говорит контейнеру:
// "Найди нужные компоненты в сцене и зарегистрируй их"
public class PlayerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerMovement>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();

        Container.Bind<PlayerHealth>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();
    }
}