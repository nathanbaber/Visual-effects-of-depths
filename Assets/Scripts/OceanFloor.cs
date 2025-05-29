using UnityEngine;

public class OceanFloor : MonoBehaviour
{
    public Terrain terrain;
    public GameObject waterPlane;
    public GameObject[] objectsOnTerrain;

    public Camera mainCamera;

    public ParticleSystem bubbleParticlesPrefab;  // Префаб системы частиц пузырей

    // Константная высота плоскости воды по Y
    public float waterPlaneHeight = 2f;

    public float waterPlaneDefaultSize = 10f; // размер Plane по умолчанию (10x10)

    // Параметры для пузырей
    public float bubbleDistanceFromCamera = 10f;   // Расстояние перед камерой, где будут пузырьки
    public float bubbleHeightAboveTerrain = 2f;    // Высота пузырьков над Terrain

    private ParticleSystem bubbleParticlesInstance;

    void Start()
    {
        if (terrain == null || waterPlane == null)
        {
            Debug.LogError("Terrain или Water Plane не назначены!");
            return;
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera не назначена!");
            return;
        }

        if (bubbleParticlesPrefab == null)
        {
            Debug.LogWarning("Префаб пузырей не назначен — пузыри не будут созданы.");
        }

        TerrainData terrainData = terrain.terrainData;

        float terrainWidth = terrainData.size.x;
        float terrainLength = terrainData.size.z;

        // Масштабируем Plane так, чтобы она покрывала Terrain по X и Z
        float scaleX = terrainWidth / waterPlaneDefaultSize;
        float scaleZ = terrainLength / waterPlaneDefaultSize;

        waterPlane.transform.localScale = new Vector3(scaleX, 1f, scaleZ);

        // Позиция Terrain — нижний левый угол (в мире)
        Vector3 terrainPos = terrain.transform.position;

        // Ставим Plane по центру Terrain, на фиксированной высоте waterPlaneHeight (относительно мира)
        waterPlane.transform.position = new Vector3(
            terrainPos.x + terrainWidth / 2f,
            waterPlaneHeight,
            terrainPos.z + terrainLength / 2f);

        // Перемещаем и ставим объекты на поверхность Terrain по высоте
        foreach (var obj in objectsOnTerrain)
        {
            if (obj == null) continue;

            Vector3 pos = obj.transform.position;

            // Получаем высоту Terrain под точкой X,Z (относительно Terrain transform.position)
            float terrainHeightAtPos = terrain.SampleHeight(pos) + terrainPos.y;

            // Обновляем позицию объекта - ставим на поверхность Terrain
            obj.transform.position = new Vector3(pos.x, terrainHeightAtPos, pos.z);
        }

        Debug.Log("Подводная сцена корректно настроена с учетом рельефа Terrain");

        // Создаём систему пузырей
        CreateBubbleParticles();
    }

    void CreateBubbleParticles()
    {
        if (bubbleParticlesPrefab == null)
            return;

        // Позиция камеры и направление её взгляда
        Vector3 camPos = mainCamera.transform.position;
        Vector3 camForward = mainCamera.transform.forward;

        // Позиция для пузырей — чуть впереди камеры
        Vector3 spawnPos = camPos + camForward * bubbleDistanceFromCamera;

        // Получаем высоту Terrain под точкой spawnPos и добавляем offset по высоте
        float terrainHeight = terrain.SampleHeight(spawnPos) + terrain.transform.position.y;
        spawnPos.y = terrainHeight + bubbleHeightAboveTerrain;

        // Создаём частицы
        bubbleParticlesInstance = Instantiate(bubbleParticlesPrefab, spawnPos, Quaternion.identity);

        // Если хотите, можно привязать к OceanFloor, чтобы удобно было удалять/отключать
        bubbleParticlesInstance.transform.parent = this.transform;

        // Запускаем систему частиц
        bubbleParticlesInstance.Play();
    }
}