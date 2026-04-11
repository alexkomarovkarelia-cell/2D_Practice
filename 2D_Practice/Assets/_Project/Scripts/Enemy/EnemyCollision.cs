using UnityEngine;
using GameCore.Health; // Нужен, если PlayerHealth находится в namespace GameCore.Health

namespace Enemy
{
    // Этот скрипт должен висеть на враге
    // Он срабатывает, когда враг входит в trigger другого объекта
    public class EnemyCollision : MonoBehaviour
    {
        [SerializeField] private float _damage = 1f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Проверяем: есть ли на объекте, в который мы вошли, компонент PlayerHealth
            if (other.gameObject.TryGetComponent(out PlayerHealth player))
            {
                // Наносим игроку урон
                player.TakeDamage(_damage);

               // player.OnHealthChanged?.Invoke();

                // Выключаем врага после столкновения
                gameObject.SetActive(false);
            }
        }
    }
}