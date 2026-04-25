using System.Collections;
using UnityEngine;
using Zenject;

// Спавнер врагов.
// Его задача:
// 1) через интервал брать врага из пула,
// 2) ставить его в случайную точку,
// 3) повторять это по кругу.
public class EnemySpawner : MonoBehaviour, IActivate
{
    [Header("Spawn Settings")]
    [SerializeField] private float _timeToSpawn = 2f;      // Время между спавнами
    [SerializeField] private Transform _minPos;            // Нижняя левая граница
    [SerializeField] private Transform _maxPos;            // Верхняя правая граница
    [SerializeField] private Transform _enemyContainer;    // Контейнер для врагов
    [SerializeField] private ObjectPool _enemyPool;        // Пул врагов
    [SerializeField] private PlayerMovement _playerMovement; // Игрок

    private WaitForSeconds _interval;                      // Кэш ожидания
    private Coroutine _spawnCoroutine;                     // Ссылка на корутину

    // Эту ссылку НЕ назначаем руками в Inspector.
    // Её передаст Zenject через Construct().
    private GetRandomSpawnPoint _getRandomSpawnPoint;

    // Inject-метод.
    // Zenject сам вызовет его и передаст сюда нужный объект.
    [Inject]
    private void Construct(GetRandomSpawnPoint getRandomSpawnPoint)
    {
        _getRandomSpawnPoint = getRandomSpawnPoint;
    }

    private void Start()
    {
        // Создаём ожидание один раз
        _interval = new WaitForSeconds(_timeToSpawn);

        // Запускаем спавн
        Activate();
    }

    public void Activate()
    {
        // Если уже работает — второй раз не запускаем
        if (_spawnCoroutine != null)
            return;

        _spawnCoroutine = StartCoroutine(Spawn());
    }

    public void Deactivate()
    {
        // Если корутина не работает — выходим
        if (_spawnCoroutine == null)
            return;

        StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = null;
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            // Подвигаем спавнер к игроку,
            // чтобы зона спавна была вокруг него
            if (_playerMovement != null)
            {
                transform.position = _playerMovement.transform.position;
            }

            // Берём врага из пула
            GameObject enemy = _enemyPool.GetFromPool();

            // Если контейнер есть — кладём врага туда
            if (_enemyContainer != null)
            {
                enemy.transform.SetParent(_enemyContainer);
            }

            // Получаем случайную точку через injected-класс
            enemy.transform.position = _getRandomSpawnPoint.GetRandomPoint(_minPos, _maxPos);

            // Ждём до следующего спавна
            yield return _interval;
        }
    }
}