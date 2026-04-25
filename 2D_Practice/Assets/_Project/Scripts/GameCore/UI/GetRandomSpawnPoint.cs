using UnityEngine;

// Это обычный вспомогательный класс.
// Он НЕ висит на объекте в сцене.
// Его задача — просто посчитать случайную точку спавна.
public class GetRandomSpawnPoint
{
    // Метод получает две точки:
    // minPos = нижняя левая граница
    // maxPos = верхняя правая граница
    public Vector3 GetRandomPoint(Transform minPos, Transform maxPos)
    {
        // Здесь соберём итоговую точку спавна
        Vector3 spawnPoint = Vector3.zero;

        // Случайно выбираем:
        // true  -> спавн по левой или правой стороне
        // false -> спавн по верхней или нижней стороне
        bool verticalSpawn = Random.Range(0f, 1f) > 0.5f;

        if (verticalSpawn)
        {
            // Спавн по вертикальным сторонам:
            // Y случайный внутри диапазона
            spawnPoint.y = Random.Range(minPos.position.y, maxPos.position.y);

            // X либо слева, либо справа
            spawnPoint.x = Random.Range(0f, 1f) > 0.5f
                ? minPos.position.x
                : maxPos.position.x;
        }
        else
        {
            // Спавн по горизонтальным сторонам:
            // X случайный внутри диапазона
            spawnPoint.x = Random.Range(minPos.position.x, maxPos.position.x);

            // Y либо снизу, либо сверху
            spawnPoint.y = Random.Range(0f, 1f) > 0.5f
                ? minPos.position.y
                : maxPos.position.y;
        }

        // Для 2D держим Z в нуле
        spawnPoint.z = 0f;

        return spawnPoint;
    }
}