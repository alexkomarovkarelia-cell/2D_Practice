using UnityEngine;
using System.Collections;      // Нужен для IEnumerator и корутин
using GameCore.Health;        // Нужен, потому что ObjectHealth лежит в этом namespace

namespace Enemy
{
    // EnemyHealth наследуется от ObjectHealth
    // Значит враг уже получает базовую систему здоровья:
    // max health, current health, TakeDamage, TakeHeal и т.д.
    public class EnemyHealth : ObjectHealth
    {
        // Задержка между тиками горения
        // То есть урон от огня будет проходить раз в 1 секунду
        private WaitForSeconds _tick = new WaitForSeconds(1f);

        // Переопределяем базовый метод получения урона
        public override void TakeDamage(float damage)
        {
            // Сначала вызываем логику из базового класса ObjectHealth
            // Там уменьшается текущее здоровье
            base.TakeDamage(damage);

            // Если здоровье закончилось — выключаем объект врага
            // gameObject — это текущий объект, на котором висит скрипт
            if (CurrentHealth <= 0)
                gameObject.SetActive(false);
        }

        // Метод запуска горения
        // damage — это общий урон от эффекта горения
        public void Burn(float damage)
        {
            // Запускаем корутину, которая будет наносить урон частями
            StartCoroutine(StartBurn(damage));
        }

        // Корутина горения
        // Она будет наносить урон не сразу весь, а по частям через время
        private IEnumerator StartBurn(float damage)
        {
            // Если объект уже выключен, то ничего не делаем
            if (!gameObject.activeSelf)
                yield break;

            // Количество тиков горения
            // То есть сколько раз будет нанесён урон
            int burnTicks = 5;

            // Делим общий урон на количество тиков
            // Например:
            // damage = 10
            // burnTicks = 5
            // значит по 2 урона за тик
            float tickDamage = damage / burnTicks;

            // Чтобы урон не получился слишком маленьким,
            // задаём минимальный урон 1 за тик
            if (tickDamage < 1f)
                tickDamage = 1f;

            // Округляем значение, чтобы было аккуратнее
            float roundDamage = Mathf.Round(tickDamage);

            // Цикл: наносим урон несколько раз
            for (int i = 0; i < burnTicks; i++)
            {
                // Если враг уже выключен в процессе горения — выходим
                if (!gameObject.activeSelf)
                    yield break;

                // Наносим урон врагу
                TakeDamage(roundDamage);

                // Ждём 1 секунду до следующего тика
                yield return _tick;
            }
        }
    }
}