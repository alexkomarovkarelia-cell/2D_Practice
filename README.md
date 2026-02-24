# 2D Practice (Brunoyam course)

Учебный 2D-проект по курсу (практика). Реализованы: Tilemap-уровень, движение, анимации, коллизии стен, декор без коллизий, выбор управления (клавиатура/джойстик).

## Unity Version
- Unity:  6....
##  Как открыть проект
1. Открой **Unity Hub**
2. Нажми **Add / Open**
3. Выбери папку: **`2D_Practice/`** (внутри репозитория)
   - В этой папке должны быть: `Assets`, `Packages`, `ProjectSettings`
## Управление
### Keyboard (New Input System)
- Движение: **WASD / стрелки**
- Interact: **R**

### Mobile Joystick (Joystick Pack)
- Движение: **Fixed Joystick** на Canvas
- Режим управления переключается в инспекторе у Player:
  - `Control Mode`: Keyboard / Joystick / Auto

## Что сделано по практике
### Easy
- Импорт и нарезка спрайтов через Sprite Editor
- Tilemap поле
- Движение персонажа в 4 направления (скорость через поле)
- Idle/Run анимации, параметры: `Speed`, `Horizontal`, `Vertical`

### Middle
- Разделение Tilemap: `Ground` (фон), `Walls` (коллизии), `Decor` (без коллизий)
- Переходы Idle ↔ Movement по `Speed`
- Blend Tree (2D Simple Directional) по `Horizontal`/`Vertical`

### Hard
- Выбор управления: Keyboard / Joystick / Auto
- Использование `joystick.Horizontal` / `joystick.Vertical` для движения
- Добавлено состояние `Interact` (Trigger) + переходы (Any State → Interact → Idle)

## Примечания
- Коллизии есть только на `Walls`. На `Decor` коллайдера нет.
- Для top-down у игрока включено `Freeze Rotation` (чтобы не переворачивался у стен).
