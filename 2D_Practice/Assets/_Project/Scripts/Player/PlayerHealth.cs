using GameCore.Health;
using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : ObjectHealth
{
    [SerializeField] private float _regenerationDelay = 5f;
    [SerializeField] private float _regenerationValue = 1f;

    // Событие: UI и другие системы могут подписаться
    // и узнавать, когда здоровье изменилось
    public Action OnHealthChanged;

    private Coroutine _regenerationCoroutine;

    private void Start()
    {
        _regenerationCoroutine = StartCoroutine(Regeneration());

        // Один раз сообщаем UI текущее здоровье при старте
        OnHealthChanged?.Invoke();
    }

    public void Heal(float value)
    {
        TakeHeal(value);
        OnHealthChanged?.Invoke();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        OnHealthChanged?.Invoke();

        if (CurrentHealth <= 0f)
        {
            Debug.Log("Игрок умер");

            if (_regenerationCoroutine != null)
            {
                StopCoroutine(_regenerationCoroutine);
                _regenerationCoroutine = null;
            }
        }
    }

    private IEnumerator Regeneration()
    {
        while (CurrentHealth > 0f)
        {
            yield return new WaitForSeconds(_regenerationDelay);

            // Сначала лечим
            TakeHeal(_regenerationValue);

            // Потом сообщаем UI, что здоровье изменилось
            OnHealthChanged?.Invoke();
        }
    }
}