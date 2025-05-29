using UnityEngine;

public class OceanFloor : MonoBehaviour
{
    [Header("Ссылки на объекты")]
    public Terrain terrain;                 // Ссылка на Terrain
    public GameObject waterPlane;           // Водная плоскость (Plane)
    public GameObject[] objectsOnTerrain;   // Объекты, которые нужно разместить на дне

    [Header("Настройки")]
    public float waterHeight = 2f;          // Высота расположения воды над Terrain
    public float waterPlaneDefaultSize = 10f;  // Размер Plane по умолчанию

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain не назначен!");
            return;
        }

        if (waterPlane == null)
        {
            Debug.LogError("Water Plane не назначена!");
            return;
        }

        // Настраиваем позицию Terrain (по желанию можно фиксировать на 0,0,0)
        terrain.transform.position = Vector3.zero;

        // Получаем размеры Terrain
        TerrainData terrainData = terrain.terrainData;
        float terrainWidth = terrainData.size.x;
        float terrainLength = terrainData.size.z;

        // Масштабируем Plane, чтобы покрыть Terrain
        float scaleX = terrainWidth / waterPlaneDefaultSize;
        float scaleZ = terrainLength / waterPlaneDefaultSize;
        waterPlane.transform.localScale = new Vector3(scaleX, 1, scaleZ);

        // Рассчитываем центр Terrain (нижний левый угол + половина размера)
        Vector3 terrainCenter = terrain.transform.position + new Vector3(terrainWidth / 2f, 0, terrainLength / 2f);

        // Позиционируем Plane по центру Terrain, на высоту waterHeight (относительно Terrain Y)
        float waterY = terrain.transform.position.y + waterHeight;
        waterPlane.transform.position = new Vector3(terrainCenter.x, waterY, terrainCenter.z);

        // Позиционируем объекты на поверхности Terrain (направляем Y на высоту Terrain)
        foreach (GameObject obj in objectsOnTerrain)
        {
            if (obj == null) continue;

            Vector3 pos = obj.transform.position;

            // Получаем высоту Terrain в XZ-позиции объекта
            float terrainHeightAtPos = terrain.SampleHeight(pos) + terrain.transform.position.y;

            // Устанавливаем объект на поверхность Terrain
            obj.transform.position = new Vector3(pos.x, terrainHeightAtPos, pos.z);
        }

        Debug.Log("Подводная сцена успешно настроена!");
    }
}
