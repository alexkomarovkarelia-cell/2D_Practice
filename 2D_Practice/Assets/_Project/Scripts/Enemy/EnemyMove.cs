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

        [Header("На каком расстоянии враг останавливается перед игроком")]
        [SerializeField] private float stopDistance = 0.8f;

        [Header("Аниматор врага")]
        [SerializeField] private Animator animator;

        [Header("Через сколько секунд проверять дистанцию до игрока")]
        [SerializeField] private float checkDelay = 3f;

        [Header("Если игрок дальше этой дистанции, враг выключается")]
        [SerializeField] private float hideDistance = 20f;

        private Rigidbody2D rb;
        private PlayerMovement player;
        private WaitForSeconds checkTime;

        /// <summary>
        /// Таймер временной остановки движения врага.
        /// Пока значение больше 0, враг не двигается.
        /// </summary>
        private float freezeTimer = 0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            // Если забыли назначить Animator в Inspector,
            // попробуем найти его у дочернего объекта.
            if (animator == null)
                animator = GetComponentInChildren<Animator>();

            checkTime = new WaitForSeconds(checkDelay);
        }

        private void Start()
        {
            StartCoroutine(CheckDistanceToHide());
        }

        private void FixedUpdate()
        {
            if (player == null) return;

            // Если враг временно "заморожен" — не двигается
            if (freezeTimer > 0f)
            {
                freezeTimer -= Time.fixedDeltaTime;

                if (animator != null)
                    animator.SetFloat("Speed", 0f);

                return;
            }

            MoveToPlayer();
        }

        private void MoveToPlayer()
        {
            Vector2 enemyPos = rb.position;
            Vector2 playerPos = player.transform.position;

            Vector2 toPlayer = playerPos - enemyPos;
            float distance = toPlayer.magnitude;

            // Если враг подошёл достаточно близко — останавливаемся
            if (distance <= stopDistance)
            {
                if (animator != null)
                    animator.SetFloat("Speed", 0f);

                return;
            }

            Vector2 dir = toPlayer.normalized;
            Vector2 nextPos = enemyPos + dir * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(nextPos);

            UpdateAnimator(dir);
        }

        private void UpdateAnimator(Vector2 dir)
        {
            if (animator == null) return;

            float horizontal = 0f;
            float vertical = 0f;

            // Для 4-направленной анимации выбираем главную ось
            if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
            {
                vertical = dir.y > 0 ? 1f : -1f;
            }
            else
            {
                horizontal = dir.x > 0 ? 1f : -1f;
            }

            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);
            animator.SetFloat("Speed", 1f);
        }

        private IEnumerator CheckDistanceToHide()
        {
            while (true)
            {
                if (player != null)
                {
                    float distance = Vector2.Distance(transform.position, player.transform.position);

                    if (distance > hideDistance)
                        gameObject.SetActive(false);
                }

                yield return checkTime;
            }
        }

        // Этот метод на будущее
        // например, чтобы остановить врага во время атаки
        public void FreezeMove(float time)
        {
            freezeTimer = time;
        }

        [Inject]
        private void Construct(PlayerMovement playerMovement)
        {
            player = playerMovement;
        }
    }
}