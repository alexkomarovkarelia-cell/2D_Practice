using UnityEngine;
using Zenject;

// Installer нужен для регистрации зависимостей в Zenject / Extenject
public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Регистрируем GetRandomSpawnPoint как одиночный объект (AsSingle)
        // FromNew() = создать новый экземпляр класса
        // NonLazy() = создать сразу при запуске, а не ждать первого запроса
        Container.Bind<GetRandomSpawnPoint>().FromNew().AsSingle().NonLazy();
    }
}