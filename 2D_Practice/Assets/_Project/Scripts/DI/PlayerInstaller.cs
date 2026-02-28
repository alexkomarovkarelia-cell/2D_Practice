using UnityEngine;
using Zenject;

// Этот инсталлер говорит контейнеру:
// "Найди PlayerMovement в сцене и сделай его Single"
public class PlayerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Ищет компонент PlayerMovement в сцене (в иерархии)
        // Очень удобно: ничего не надо перетаскивать в Inspector
        Container.Bind<PlayerMovement>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();
    }
}