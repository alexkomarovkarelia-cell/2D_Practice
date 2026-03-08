using System.Collections;
using UnityEngine;
using Zenject;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMove : MonoBehaviour
    {
        [Header("Скорость движения врага")]
        [SerializeField] private float moveSpeed = 2f;

        [Header("Если freezeTimer > 0, враг временно не двигается")]
        [SerializeField] private float freezeTimer = 0f;

        [Header("Аниматор врага (если есть)")]
        [SerializeField] private Animator animator;

        [Header("Через сколько секунд проверять дистанцию до игрока")]
        [SerializeField] private float checkDelay = 3f;

        [Header("Если игрок дальше этой дистанции, враг выключается")]
        [SerializeField] private float hideDistance = 20f;

        private Rigidbody2D rb;
        private PlayerMovement player;

        private WaitForSeconds checkTime;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            // Создаём объект ожидания один раз, чтобы не создавать его каждый цикл
            checkTime = new WaitForSeconds(checkDelay);
        }

        private void Start()
        {
            // Запускаем корутину проверки расстояния
            StartCoroutine(CheckDistanceToHide());
        }

        private void FixedUpdate()
        {
            // Если игрок не найден — ничего не делаем
            if (player == null) return;

            // Если враг "заморожен", уменьшаем таймер и не двигаемся
            if (freezeTimer > 0f)
            {
                freezeTimer -= Time.fixedDeltaTime;
                return;
            }

            MoveToPlayer();
        }

        private void MoveToPlayer()
        {
            Vector2 enemyPos = rb.position;
            Vector2 playerPos = player.transform.position;

            // Направление от врага к игроку
            Vector2 dir = (playerPos - enemyPos).normalized;

            // Следующая позиция врага
            Vector2 nextPos = enemyPos + dir * moveSpeed * Time.fixedDeltaTime;

            // Двигаем Rigidbody2D
            rb.MovePosition(nextPos);

            // Если аниматор подключён — можно обновлять параметры
            if (animator != null)
            {
                animator.SetFloat("Horizontal", dir.x);
                animator.SetFloat("Vertical", dir.y);
                animator.SetFloat("Speed", dir.magnitude);
            }
        }

        private IEnumerator CheckDistanceToHide()
        {
            while (true)
            {
                // Если игрок ещё не найден — просто ждём следующую проверку
                if (player != null)
                {
                    float distance = Vector3.Distance(transform.position, player.transform.position);

                    if (distance > hideDistance)
                    {
                        gameObject.SetActive(false);
                    }
                }

                yield return checkTime;
            }
        }

        // Этот метод можно вызывать, если нужно временно остановить врага
        public void FreezeMove(float time)
        {
            freezeTimer = time;
        }

        [Inject]
        private void Construct(PlayerMovement playerMovement)
        {
            player = playerMovement;
            Debug.Log("EnemyMove injected PlayerMovement: " + (player != null));
        }
    }
}