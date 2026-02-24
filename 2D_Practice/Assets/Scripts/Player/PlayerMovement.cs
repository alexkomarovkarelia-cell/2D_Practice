using UnityEngine;
using UnityEngine.InputSystem; // New Input System (InputValue приходит из PlayerInput)

// Скрипт движения игрока + поддержка 2 источников ввода:
// 1) Keyboard (New Input System через PlayerInput -> OnMove)
// 2) Joystick Pack (Fixed/Floating/Dynamic Joystick -> joystick.Horizontal / joystick.Vertical)
//
// Цель: можно выбрать режим управления:
// - Keyboard  : только клавиатура/геймпад из InputActions
// - Joystick  : только экранный джойстик
// - Auto      : если джойстик отклонён — используем его, иначе клавиатуру
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // Какой источник управления использовать
    public enum ControlMode
    {
        Keyboard,  // только New Input System (WASD/стик геймпада и т.д.)
        Joystick,  // только экранный джойстик (Joystick Pack)
        Auto       // автоматически: джойстик имеет приоритет, если его тронули
    }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;           // скорость движения (настраивается в инспекторе)
    [SerializeField] private ControlMode controlMode = ControlMode.Auto; // режим управления (можно менять в инспекторе)

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;                 // Rigidbody2D игрока  найдём сами если нет
    [SerializeField] private Animator animator;              // Animator (обычно на дочернем объекте Player_0)

    // Joystick Pack:
    // В инспекторе нужно перетащить (FixedJoystick / FloatingJoystick / DynamicJoystick).
    // Важно: тип поля "Joystick" — это базовый класс из Joystick Pack.
    [Header("Joystick Pack (optional)")]
    [SerializeField] private Joystick joystick;
    [SerializeField, Range(0f, 1f)] private float joystickDeadZone = 0.2f; // убирает дрожь у центра
    [SerializeField] private bool joystickDigital = true; // true = всегда полный бег (как клавиши)

    // ВНУТРЕННЕЕ СОСТОЯНИЕ СКРИПТА


    // Ввод с клавиатуры / геймпада из New Input System (приходит в OnMove)
    private Vector2 inputKeyboard;

    // Последнее НЕнулевое направление движения.
    // Нужно, чтобы в Idle персонаж "смотрел" в последнюю сторону, а не сбрасывался на (0,0).
    private Vector2 lastMoveDir = Vector2.down;

    private void Awake()
    {
        // 1) Забираем Rigidbody2D, если не назначен вручную
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        // 2) Для top-down обычно гравитация не нужна
        rb.gravityScale = 0f;

        // 3) Чтобы физика не крутила игрока при столкновениях со стенами
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 4) Забираем Animator, если не назначен вручную Он на Player_0 чаще всего
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    
    // NEW INPUT SYSTEM: PlayerInput вызывает OnMove(...)
       // Чтобы это работало:
    // - На игроке должен быть компонент PlayerInput
    // - Behavior: Send Messages
    // - Action "Move" должна быть в InputActions
    // - Название callback должно совпадать: OnMove
    public void OnMove(InputValue value)
    {
        // Считываем Vector2: (x,y)
        // x: влево(-1) / вправо(+1)
        // y: вниз(-1) / вверх(+1)
        inputKeyboard = value.Get<Vector2>();

        // Нормализация нужна, чтобы по диагонали не было быстрее:
        // (1,1) по длине больше чем (1,0), поэтому скорость была бы выше.
        if (inputKeyboard.sqrMagnitude > 1f)
            inputKeyboard.Normalize();
    }

    private void FixedUpdate()
    {
        // В физике (FixedUpdate) мы двигаем Rigidbody2D
        // Здесь выбираем, какой ввод использовать: клавиатура/джойстик/авто
        Vector2 move = GetMoveInput();

        // Движение:
        // Линейная скорость = направление * скорость
        rb.linearVelocity = move * moveSpeed;

        // Обновляем параметры анимации (направление и скорость)
        UpdateAnimator(move);
    }

    
    // ВЫБОР ИСТОЧНИКА УПРАВЛЕНИЯ
   
    private Vector2 GetMoveInput()
    {
        // 1) Получаем ввод с джойстика (если он назначен)
        Vector2 joy = Vector2.zero;

        if (joystick != null)
        {
            joy = new Vector2(joystick.Horizontal, joystick.Vertical);

            // 1) DeadZone — гасим мелкий шум возле нуля
            if (joy.magnitude < joystickDeadZone)
            {
                joy = Vector2.zero;
            }
            else
            {
                // 2) Digital режим — делаем как WASD: длина всегда 1
                // Нормализуем диагональ (на всякий)
                if (joystickDigital)
                    joy = joy.normalized;
                else if (joy.sqrMagnitude > 1f)
                    joy.Normalize();
            }

       
        }

        // 2) Выбираем режим
        switch (controlMode)
        {
            case ControlMode.Keyboard:
                // Только клавиатура/геймпад из New Input System
                return inputKeyboard;

            case ControlMode.Joystick:
                // Только экранный джойстик
                return joy;

            case ControlMode.Auto:
            default:
                // Авто: если джойстик реально отклонён — используем его,
                // иначе используем клавиатуру.
                // Порог 0.01f — чтобы игнорировать мелкий шум/дрожание джойстика.
                return (joy != Vector2.zero) ? joy : inputKeyboard;
        }
    }

    
    // АНИМАЦИЯ (Blend Tree параметры)
   
    // Предполагается, что в Animator есть float параметры:
    // "Horizontal", "Vertical", "Speed"
    //
    // Blend Tree:
    // - 2D Simple Directional
    // - X = Horizontal, Y = Vertical
    // - RunRight (1,0), RunLeft (-1,0), RunUp (0,1), RunDown (0,-1)
    private void UpdateAnimator(Vector2 move)
    {
        // Если Animator не найден — просто выходим (движение будет работать без анимаций)
        if (animator == null) return;

        // Если игрок движется — обновляем lastMoveDir
        if (move.sqrMagnitude > 0.001f)
            lastMoveDir = move;

        // Направление для Blend Tree берём из lastMoveDir,
        // чтобы когда move = (0,0) (стоим), персонаж не "терял" ориентацию.
        animator.SetFloat("Horizontal", lastMoveDir.x);
        animator.SetFloat("Vertical", lastMoveDir.y);

        // Speed используем для перехода Idle <-> Movement
       
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