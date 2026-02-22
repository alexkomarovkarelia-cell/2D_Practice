


using UnityEngine;
using UnityEngine.InputSystem; //Новая система New Input System

// Движение игрока в 2D (вид сверху) через New Input System + PlayerInput (Send Messages)
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f; // скорость (юнитов в секунду)

    [Header("Animation")]
    [SerializeField] private Animator animator; // перетащи сюда Animator (обычно на дочернем PlayerSprite)

    private Rigidbody2D rb;
    private Vector2 moveInput; // сюда приходит ввод (x,y)

    // запоминаем последнее направление, чтобы в Idle персонаж "смотрел" туда же
    private Vector2 lastMoveDir = Vector2.down;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Для топ-даун движения обычно гравитация не нужна
        rb.gravityScale = 0f;

        // если забыли назначить animator в инспекторе — попробуем найти на детях
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
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
        // если есть ввод — запоминаем направление
        if (moveInput.sqrMagnitude > 0.001f)
            lastMoveDir = moveInput;

        UpdateAnimator();

    }

    private void FixedUpdate()
    {
        // Двигаем через физику (так стабильнее)
        rb.linearVelocity = moveInput * moveSpeed;
    }
    private void UpdateAnimator()
    {
        if (animator == null) return;

        // В Blend Tree обычно подают направление (Horizontal/Vertical)
        // а Speed используют для перехода Idle <-> Movement
        animator.SetFloat("Horizontal", lastMoveDir.x);
        animator.SetFloat("Vertical", lastMoveDir.y);
        animator.SetFloat("Speed", moveInput.sqrMagnitude); // 0 стоим, >0 идём
    }
}
