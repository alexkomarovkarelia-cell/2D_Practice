using UnityEngine;
using Zenject;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMove : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;

        private Rigidbody2D rb;
        private PlayerMovement player;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (player == null) return;

            Vector2 enemyPos = rb.position;
            Vector2 playerPos = player.transform.position;

            Vector2 dir = (playerPos - enemyPos).normalized;
            Vector2 nextPos = enemyPos + dir * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(nextPos);
        }

        [Inject]
        private void Construct(PlayerMovement playerMovement)
        {
            player = playerMovement;
            Debug.Log("EnemyMove injected PlayerMovement: " + (player != null));
        }
    }
}