using UnityEngine;

public class OceanFloor : MonoBehaviour
{
    [Header("������ �� �������")]
    public Terrain terrain;                 // ������ �� Terrain
    public GameObject waterPlane;           // ������ ��������� (Plane)
    public GameObject[] objectsOnTerrain;   // �������, ������� ����� ���������� �� ���

    [Header("���������")]
    public float waterHeight = 2f;          // ������ ������������ ���� ��� Terrain
    public float waterPlaneDefaultSize = 10f;  // ������ Plane �� ���������

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain �� ��������!");
            return;
        }

        if (waterPlane == null)
        {
            Debug.LogError("Water Plane �� ���������!");
            return;
        }

        // ����������� ������� Terrain (�� ������� ����� ����������� �� 0,0,0)
        terrain.transform.position = Vector3.zero;

        // �������� ������� Terrain
        TerrainData terrainData = terrain.terrainData;
        float terrainWidth = terrainData.size.x;
        float terrainLength = terrainData.size.z;

        // ������������ Plane, ����� ������� Terrain
        float scaleX = terrainWidth / waterPlaneDefaultSize;
        float scaleZ = terrainLength / waterPlaneDefaultSize;
        waterPlane.transform.localScale = new Vector3(scaleX, 1, scaleZ);

        // ������������ ����� Terrain (������ ����� ���� + �������� �������)
        Vector3 terrainCenter = terrain.transform.position + new Vector3(terrainWidth / 2f, 0, terrainLength / 2f);

        // ������������� Plane �� ������ Terrain, �� ������ waterHeight (������������ Terrain Y)
        float waterY = terrain.transform.position.y + waterHeight;
        waterPlane.transform.position = new Vector3(terrainCenter.x, waterY, terrainCenter.z);

        // ������������� ������� �� ����������� Terrain (���������� Y �� ������ Terrain)
        foreach (GameObject obj in objectsOnTerrain)
        {
            if (obj == null) continue;

            Vector3 pos = obj.transform.position;

            // �������� ������ Terrain � XZ-������� �������
            float terrainHeightAtPos = terrain.SampleHeight(pos) + terrain.transform.position.y;

            // ������������� ������ �� ����������� Terrain
            obj.transform.position = new Vector3(pos.x, terrainHeightAtPos, pos.z);
        }

        Debug.Log("��������� ����� ������� ���������!");
    }
}
