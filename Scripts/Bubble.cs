using UnityEngine;

public class Bubble : MonoBehaviour
{
    // �������� �������
    public float riseSpeedMin = 0.5f;
    public float riseSpeedMax = 1.5f;
    private float riseSpeed;

    // ��������� � ������� ��������� �� X
    public float amplitude = 0.1f;
    public float frequency = 1f;

    // ������� �������� ������
    public float rotationSpeed = 30f;

    // ������ ����������� ����, �� ������� ������ ��������
    public float surfaceHeight = 5f;

    private Vector3 startPosition;
    private float elapsedTime;

    void Start()
    {
        // ����� ��������� �������� ������� � �������� ���������
        riseSpeed = Random.Range(riseSpeedMin, riseSpeedMax);
        startPosition = transform.position;
        elapsedTime = 0f;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // ������ ����������� �� Y � ���������� ���������
        float newY = transform.position.y + riseSpeed * Time.deltaTime;

        // ��������� �� X � ����������
        float newX = startPosition.x + amplitude * Mathf.Sin(frequency * elapsedTime * 2 * Mathf.PI);

        // ������� Z ��� ���������
        float newZ = transform.position.z;

        // ��������� �������
        transform.position = new Vector3(newX, newY, newZ);

        // ������� ������ ������ ����� ��� Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // ���� �������� ����������� ����, ���������� ������
        if (transform.position.y >= surfaceHeight)
        {
            Destroy(gameObject);
        }
    }
}