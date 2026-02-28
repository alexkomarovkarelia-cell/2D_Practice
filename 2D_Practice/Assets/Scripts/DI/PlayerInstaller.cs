using UnityEngine;
using Zenject; // В Extenject namespace всё равно обычно Zenject

// PlayerInstaller — это "место, где мы говорим DI-контейнеру:
// 'вот этот PlayerMovement — один-единственный в сцене, отдавай его всем кто попросит'".
public class PlayerInstaller : MonoInstaller
{
    [Header("Ссылка на PlayerMovement из сцены")]
    [SerializeField] private PlayerMovement playerMovement;

    public override void InstallBindings()
    {
        // Защита от забывчивости: если не назначил ссылку в инспекторе,
        // лучше сразу понятная ошибка.
        if (playerMovement == null)
        {
            Debug.LogError("PlayerInstaller: не назначен playerMovement в Inspector!");
            return;
        }

        // Bind = зарегистрировать тип в контейнере
        // FromInstance = использовать именно этот объект из сцены
        // AsSingle = как 'одиночка' (по сути один экземпляр на контейнер)
        Container.Bind<PlayerMovement>()
            .FromInstance(playerMovement)
            .AsSingle();

        // ВАЖНО:
        // FromInstance НЕ всегда автоматически "инжектит" зависимости внутрь этого объекта.
        // Если в PlayerMovement есть поля [Inject], добавь это:
        Container.QueueForInject(playerMovement);

        // NonLazy обычно используют, когда надо "создать" объект на старте.
      
      
    }
}