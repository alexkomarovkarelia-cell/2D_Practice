using UnityEngine;

namespace GameCore.Health
{
    public abstract class ObjectHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHealth = 10f;
        [SerializeField] private float _currentHealth;

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public bool IsDead => _currentHealth <= 0f;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public virtual void TakeDamage(float damage)
        {
            if (damage <= 0f || IsDead)
                return;

            _currentHealth -= damage;

            if (_currentHealth < 0f)
                _currentHealth = 0f;

            if (_currentHealth <= 0f)
                Die();
        }

        public virtual void TakeHeal(float value)
        {
            if (value <= 0f || IsDead)
                return;

            _currentHealth += value;

            if (_currentHealth > _maxHealth)
                _currentHealth = _maxHealth;
        }

        protected virtual void Die()
        {
        }
    }
}