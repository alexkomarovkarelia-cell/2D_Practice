using UnityEngine;
using UnityEngine.InputSystem; // New Input System (PlayerInput, InputValue, InputAction)

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public enum ControlMode
    {
        Keyboard,   // только New Input System (клава/геймпад)
        Joystick,   // только экранный джойстик (Joystick Pack)
        Auto        // если джойстик отклонён — он, иначе клавиатура
    }

    [Header("Movement (базовое движение)")]
    [SerializeField] private float moveSpeed = 5f;                 // базовая скорость
    [SerializeField] private ControlMode controlMode = ControlMode.Auto;

    [Header("Sprint / Acceleration (ускорение/спринт)")]
    [SerializeField] private float sprintMultiplier = 1.7f;        // во сколько раз быстрее при спринте
    [SerializeField] private bool sprintToggle = false;            // false = удержание, true = переключатель
    [SerializeField] private float speedChangePerSecond = 25f;      // 0 = мгновенно, >0 = плавно

    [Header("References (ссылки на компоненты)")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInput playerInput;              // ✅ важно: чтобы читать Sprint.IsPressed()

    [Header("Joystick Pack (экранный джойстик, опционально)")]
    [SerializeField] private Joystick joystick;
    [SerializeField, Range(0f, 1f)] private float joystickDeadZone = 0.2f;
    [SerializeField] private bool joystickDigital = true;

    // ====== ВНУТРЕННИЕ ПЕРЕМЕННЫЕ ======

    // Ввод движения из New Input System (WASD/стик геймпада)
    private Vector2 inputKeyboard;

    // Последнее направление, чтобы персонаж "смотрел" в сторону последнего движения
    private Vector2 lastMoveDir = Vector2.down;

    // Спринт для режима toggle (вкл/выкл)
    private bool sprintActive = false;

    // Удержание спринта от UI-кнопки (для мобилки)
    private bool sprintUIHeld = false;

    // Текущая скорость (для плавного разгона)
    private float currentSpeed;

    // ✅ Ссылка на action Sprint (чтобы читать IsPressed каждый кадр)
    private InputAction sprintAction;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        currentSpeed = moveSpeed;

        // Берём action Sprint по имени из InputActions.
        // ВАЖНО: это не "событие", а "состояние кнопки", которое можно спросить в любой момент.
        if (playerInput != null && playerInput.actions != null)
        {
            // throwIfNotFound = false -> если нет такого action, просто вернёт null
            sprintAction = playerInput.actions.FindAction("Sprint", false);
        }
    }

    // ============================================================
    // 1) ДВИЖЕНИЕ: вызывается PlayerInput (Send Messages) от action "Move"
    // ============================================================
    public void OnMove(InputValue value)
    {
        inputKeyboard = value.Get<Vector2>();

        // Чтобы по диагонали не было быстрее
        if (inputKeyboard.sqrMagnitude > 1f)
            inputKeyboard.Normalize();
    }

    // ============================================================
    // 2) СПРИНТ: событие нажатия (но отпускание может не приходить в Send Messages)
    // Поэтому:
    // - в режиме HOLD (sprintToggle=false) мы ИГНОРИРУЕМ OnSprint
    //   и читаем состояние кнопки через sprintAction.IsPressed() в FixedUpdate
    // - в режиме TOGGLE (sprintToggle=true) используем OnSprint как переключатель
    // ============================================================
    public void OnSprint(InputValue value)
    {
        // HOLD режим: состояние берём в FixedUpdate через IsPressed()
        if (!sprintToggle) return;

        // TOGGLE режим: нажал -> переключили
        if (value.isPressed)
            sprintActive = !sprintActive;
    }

    // ============================================================
    // 3) СПРИНТ ДЛЯ МОБИЛКИ (UI-кнопка)
    // PointerDown -> SetSprint(true)
    // PointerUp   -> SetSprint(false)
    // ============================================================
    public void SetSprint(bool pressed)
    {
        if (!sprintToggle)
        {
            // HOLD режим: пока держим кнопку на экране — бежим
            sprintUIHeld = pressed;
        }
        else
        {
            // TOGGLE режим: одно нажатие переключает
            if (pressed) sprintActive = !sprintActive;
        }
    }

    private void FixedUpdate()
    {
        Vector2 move = GetMoveInput();

        // ============================================================
        // ✅ ГЛАВНЫЙ ФИКС "залипания"
        // ============================================================
        // Если sprintToggle = false (удержание),
        // мы каждый кадр спрашиваем: кнопка Sprint сейчас зажата или нет?
        bool sprintNow;

        if (sprintToggle)
        {
            // toggle: состояние хранится в sprintActive
            sprintNow = sprintActive;
        }
        else
        {
            // hold: UI-кнопка ИЛИ физическая кнопка из InputActions
            bool inputHeld = (sprintAction != null && sprintAction.IsPressed());
            sprintNow = sprintUIHeld || inputHeld;
        }

        // Целевая скорость
        float targetSpeed = moveSpeed * (sprintNow ? sprintMultiplier : 1f);

        // Плавно меняем скорость (или мгновенно, если speedChangePerSecond = 0)
        if (speedChangePerSecond <= 0f)
        {
            currentSpeed = targetSpeed;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                targetSpeed,
                speedChangePerSecond * Time.fixedDeltaTime
            );
        }

        // Двигаем Rigidbody2D
        // ⚠️ Если Unity ругнётся на linearVelocity — замени на rb.velocity
        rb.linearVelocity = move * currentSpeed;

        UpdateAnimator(move);
    }

    private Vector2 GetMoveInput()
    {
        Vector2 joy = Vector2.zero;

        if (joystick != null)
        {
            joy = new Vector2(joystick.Horizontal, joystick.Vertical);

            // deadzone — чтобы не дрожал
            if (joy.magnitude < joystickDeadZone)
                joy = Vector2.zero;
            else
            {
                if (joystickDigital)
                    joy = joy.normalized; // как WASD — всегда полный ход
                else if (joy.sqrMagnitude > 1f)
                    joy.Normalize(); // на всякий случай
            }
        }

        switch (controlMode)
        {
            case ControlMode.Keyboard:
                return inputKeyboard;

            case ControlMode.Joystick:
                return joy;

            case ControlMode.Auto:
            default:
                return (joy != Vector2.zero) ? joy : inputKeyboard;
        }
    }

    private void UpdateAnimator(Vector2 move)
    {
        if (animator == null) return;

        if (move.sqrMagnitude > 0.001f)
            lastMoveDir = move;

        animator.SetFloat("Horizontal", lastMoveDir.x);
        animator.SetFloat("Vertical", lastMoveDir.y);

        float speedParam = (move.sqrMagnitude > 0.001f) ? 1f : 0f;
        animator.SetFloat("Speed", speedParam);
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        if (animator != null)
            animator.SetTrigger("Interact");
    }
}