using GameCore.Health;     // Подключаем наш базовый класс здоровья
using System.Collections;  // Нужен для Coroutine
using UnityEngine;         // Базовые классы Unity

public class PlayerHealth : ObjectHealth
{
    [Header("Regeneration Settings")]
    [SerializeField] private float _regenerationDelay = 5f;   // Задержка между тиками регенерации
    [SerializeField] private float _regenerationValue = 1f;   // Сколько здоровья восстанавливается за один тик

    private Coroutine _regenerationCoroutine; // Ссылка на запущенную корутину регенерации

    protected override void Start()
    {
        // Сначала вызываем Start() базового класса,
        // чтобы отправилось первое событие OnHealthChanged
        base.Start();

        // Запускаем регенерацию здоровья
        _regenerationCoroutine = StartCoroutine(Regeneration());
    }

    // Отдельный удобный метод лечения игрока
    // Например, его можно вызывать из аптечки
    public void Heal(float value)
    {
        TakeHeal(value);
    }

    public override void TakeDamage(float damage)
    {
        // Сначала выполняем обычную базовую логику получения урона:
        // - уменьшение здоровья
        // - ограничение до 0
        // - вызов OnHealthChanged
        // - проверка смерти
        base.TakeDamage(damage);

        // Если здоровье закончилось
        if (CurrentHealth <= 0f)
        {
            Debug.Log("Игрок умер");

            // Останавливаем регенерацию, если она была запущена
            if (_regenerationCoroutine != null)
            {
                StopCoroutine(_regenerationCoroutine);
                _regenerationCoroutine = null;
            }
        }
    }

    // Корутина — это метод, который может работать во времени
    private IEnumerator Regeneration()
    {
        // Пока игрок жив — регенерация работает
        while (CurrentHealth > 0f)
        {
            // Ждём указанное количество секунд
            yield return new WaitForSeconds(_regenerationDelay);

            // Лечим игрока
            // Внутри TakeHeal уже вызовется OnHealthChanged,
            // поэтому HP bar должен обновиться автоматически
            TakeHeal(_regenerationValue);
        }
    }
}