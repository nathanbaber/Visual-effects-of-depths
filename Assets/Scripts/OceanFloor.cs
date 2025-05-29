using UnityEngine;

public class OceanFloor : MonoBehaviour
{
    public Terrain terrain;
    public GameObject waterPlane;
    public GameObject[] objectsOnTerrain;

    public Camera mainCamera;

    public ParticleSystem bubbleParticlesPrefab;  // ������ ������� ������ �������

    // ����������� ������ ��������� ���� �� Y
    public float waterPlaneHeight = 2f;

    public float waterPlaneDefaultSize = 10f; // ������ Plane �� ��������� (10x10)

    // ��������� ��� �������
    public float bubbleDistanceFromCamera = 10f;   // ���������� ����� �������, ��� ����� ��������
    public float bubbleHeightAboveTerrain = 2f;    // ������ ��������� ��� Terrain

    private ParticleSystem bubbleParticlesInstance;

    void Start()
    {
        if (terrain == null || waterPlane == null)
        {
            Debug.LogError("Terrain ��� Water Plane �� ���������!");
            return;
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera �� ���������!");
            return;
        }

        if (bubbleParticlesPrefab == null)
        {
            Debug.LogWarning("������ ������� �� �������� � ������ �� ����� �������.");
        }

        TerrainData terrainData = terrain.terrainData;

        float terrainWidth = terrainData.size.x;
        float terrainLength = terrainData.size.z;

        // ������������ Plane ���, ����� ��� ��������� Terrain �� X � Z
        float scaleX = terrainWidth / waterPlaneDefaultSize;
        float scaleZ = terrainLength / waterPlaneDefaultSize;

        waterPlane.transform.localScale = new Vector3(scaleX, 1f, scaleZ);

        // ������� Terrain � ������ ����� ���� (� ����)
        Vector3 terrainPos = terrain.transform.position;

        // ������ Plane �� ������ Terrain, �� ������������� ������ waterPlaneHeight (������������ ����)
        waterPlane.transform.position = new Vector3(
            terrainPos.x + terrainWidth / 2f,
            waterPlaneHeight,
            terrainPos.z + terrainLength / 2f);

        // ���������� � ������ ������� �� ����������� Terrain �� ������
        foreach (var obj in objectsOnTerrain)
        {
            if (obj == null) continue;

            Vector3 pos = obj.transform.position;

            // �������� ������ Terrain ��� ������ X,Z (������������ Terrain transform.position)
            float terrainHeightAtPos = terrain.SampleHeight(pos) + terrainPos.y;

            // ��������� ������� ������� - ������ �� ����������� Terrain
            obj.transform.position = new Vector3(pos.x, terrainHeightAtPos, pos.z);
        }

        Debug.Log("��������� ����� ��������� ��������� � ������ ������� Terrain");

        // ������ ������� �������
        CreateBubbleParticles();
    }

    void CreateBubbleParticles()
    {
        if (bubbleParticlesPrefab == null)
            return;

        // ������� ������ � ����������� � �������
        Vector3 camPos = mainCamera.transform.position;
        Vector3 camForward = mainCamera.transform.forward;

        // ������� ��� ������� � ���� ������� ������
        Vector3 spawnPos = camPos + camForward * bubbleDistanceFromCamera;

        // �������� ������ Terrain ��� ������ spawnPos � ��������� offset �� ������
        float terrainHeight = terrain.SampleHeight(spawnPos) + terrain.transform.position.y;
        spawnPos.y = terrainHeight + bubbleHeightAboveTerrain;

        // ������ �������
        bubbleParticlesInstance = Instantiate(bubbleParticlesPrefab, spawnPos, Quaternion.identity);

        // ���� ������, ����� ��������� � OceanFloor, ����� ������ ���� �������/���������
        bubbleParticlesInstance.transform.parent = this.transform;

        // ��������� ������� ������
        bubbleParticlesInstance.Play();
    }
}