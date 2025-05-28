using UnityEngine;

public class Bubble : MonoBehaviour
{
    // Скорость подъёма
    public float riseSpeedMin = 0.5f;
    public float riseSpeedMax = 1.5f;
    private float riseSpeed;

    // Амплитуда и частота колебаний по X
    public float amplitude = 0.1f;
    public float frequency = 1f;

    // Частота вращения пузыря
    public float rotationSpeed = 30f;

    // Высота поверхности воды, на которой пузырь исчезает
    public float surfaceHeight = 5f;

    private Vector3 startPosition;
    private float elapsedTime;

    void Start()
    {
        // Задаём случайную скорость подъёма в заданном диапазоне
        riseSpeed = Random.Range(riseSpeedMin, riseSpeedMax);
        startPosition = transform.position;
        elapsedTime = 0f;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // Пузырь поднимается по Y с постоянной скоростью
        float newY = transform.position.y + riseSpeed * Time.deltaTime;

        // Колебание по X с синусоидой
        float newX = startPosition.x + amplitude * Mathf.Sin(frequency * elapsedTime * 2 * Mathf.PI);

        // Оставим Z без изменений
        float newZ = transform.position.z;

        // Обновляем позицию
        transform.position = new Vector3(newX, newY, newZ);

        // Вращаем пузырь вокруг своей оси Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Если достигли поверхности воды, уничтожаем пузырь
        if (transform.position.y >= surfaceHeight)
        {
            Destroy(gameObject);
        }
    }
}