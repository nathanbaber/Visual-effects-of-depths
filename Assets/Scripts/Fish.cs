using UnityEngine;

public class Fish : MonoBehaviour
{
    // Дополнительные параметры, которые можно настраивать в инспекторе:

    [Tooltip("Скорость анимации плавников (если используется анимация)")]
    public float swimAnimationSpeed = 1f;

    [Tooltip("Материал рыбы (если нужно менять цвет или другие параметры)")]
    public Material fishMaterial;

    private void Start()
    {
        // Получаем компонент Renderer (если он есть)
        Renderer fishRenderer = GetComponent<Renderer>();

        // Если у рыбы есть материал, сохраняем его
        if (fishRenderer != null)
        {
            fishMaterial = fishRenderer.material; // Получаем экземпляр материала
        }

        // Дополнительная инициализация (если требуется)
    }

    private void Update()
    {
        // Если у вас есть анимация, можно управлять ею здесь,
        // например, менять скорость анимации плавников:
        // GetComponent<Animator>().SetFloat("Speed", swimAnimationSpeed);

        // Дополнительные обновления (если требуется)
    }

    // Другие функции, например, для изменения цвета, если нужно.
    public void SetColor(Color color)
    {
        if (fishMaterial != null)
        {
            fishMaterial.color = color;
        }
    }
}
