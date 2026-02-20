


using UnityEngine;
using UnityEngine.InputSystem; // важно для New Input System

// Движение игрока в 2D (вид сверху) через New Input System + PlayerInput (Send Messages)
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // скорость (юнитов в секунду)

    private Rigidbody2D rb;
    private Vector2 moveInput; // сюда приходит ввод (x,y)

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Для топ-даун движения обычно гравитация не нужна
        rb.gravityScale = 0f;
    }

    // Этот метод будет автоматически вызываться PlayerInput'ом,
    // потому что в PlayerInput стоит Behavior = Send Messages
    // и Action называется "Move" => метод должен называться OnMove
    public void OnMove(InputValue value)
    {
        // Считываем Vector2 из действия Move
        moveInput = value.Get<Vector2>();

        // Нормализуем, чтобы по диагонали не было быстрее
        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();
    }

    private void FixedUpdate()
    {
        // Двигаем через физику (так стабильнее)
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
