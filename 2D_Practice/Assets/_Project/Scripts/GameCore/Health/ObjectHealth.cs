using System;          // Нужен для Action (события)
using UnityEngine;     // Базовые классы Unity

namespace GameCore.Health
{
    // abstract = это базовый класс, сам по себе обычно не вешается на объект,
    // а служит основой для других классов, например PlayerHealth, EnemyHealth и т.д.
    public abstract class ObjectHealth : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float _maxHealth = 10f;     // Максимальное здоровье объекта
        [SerializeField] private float _currentHealth;       // Текущее здоровье объекта

        // Свойства только для чтения снаружи
        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public bool IsDead => _currentHealth <= 0f;

        // Событие изменения здоровья.
        // Теперь оно находится в БАЗОВОМ классе, и любой наследник сможет его использовать.
        // Например:
        // - PlayerHealth
        // - EnemyHealth
        // - BossHealth
        // - BarrelHealth
        public event Action OnHealthChanged;

        // protected virtual:
        // protected = доступно внутри этого класса и его наследников
        // virtual = можно переопределить в дочернем классе
        protected virtual void Awake()
        {
            // При старте объекта текущее здоровье ставим равным максимальному
            _currentHealth = _maxHealth;
        }

        protected virtual void Start()
        {
            // Важно:
            // первое уведомление отправляем в Start(), а не в Awake(),
            // чтобы UI успел подписаться на событие.
            OnHealthChanged?.Invoke();
        }

        // Метод получения урона
        public virtual void TakeDamage(float damage)
        {
            // Если урон некорректный или объект уже мёртв — выходим
            if (damage <= 0f || IsDead)
                return;

            // Уменьшаем текущее здоровье
            _currentHealth -= damage;

            // Не даём здоровью уйти ниже 0
            if (_currentHealth < 0f)
                _currentHealth = 0f;

            // Сообщаем всем подписчикам:
            // "Здоровье изменилось"
            OnHealthChanged?.Invoke();

            // Если здоровье закончилось — вызываем смерть
            if (_currentHealth <= 0f)
                Die();
        }

        // Метод лечения
        public virtual void TakeHeal(float value)
        {
            // Если лечение некорректное или объект уже мёртв — выходим
            if (value <= 0f || IsDead)
                return;

            // Увеличиваем здоровье
            _currentHealth += value;

            // Не даём здоровью превысить максимум
            if (_currentHealth > _maxHealth)
                _currentHealth = _maxHealth;

            // Сообщаем подписчикам, что здоровье изменилось
            OnHealthChanged?.Invoke();
        }

        // Метод смерти.
        // Пока пустой, но наследники могут переопределить его под себя.
        // Например:
        // - игрок проиграл
        // - враг исчез
        // - объект сломался
        protected virtual void Die()
        {
        }
    }
}