using UnityEngine;

public class Fish : MonoBehaviour
{
    // �������������� ���������, ������� ����� ����������� � ����������:

    [Tooltip("�������� �������� ��������� (���� ������������ ��������)")]
    public float swimAnimationSpeed = 1f;

    [Tooltip("�������� ���� (���� ����� ������ ���� ��� ������ ���������)")]
    public Material fishMaterial;

    private void Start()
    {
        // �������� ��������� Renderer (���� �� ����)
        Renderer fishRenderer = GetComponent<Renderer>();

        // ���� � ���� ���� ��������, ��������� ���
        if (fishRenderer != null)
        {
            fishMaterial = fishRenderer.material; // �������� ��������� ���������
        }

        // �������������� ������������� (���� ���������)
    }

    private void Update()
    {
        // ���� � ��� ���� ��������, ����� ��������� �� �����,
        // ��������, ������ �������� �������� ���������:
        // GetComponent<Animator>().SetFloat("Speed", swimAnimationSpeed);

        // �������������� ���������� (���� ���������)
    }

    // ������ �������, ��������, ��� ��������� �����, ���� �����.
    public void SetColor(Color color)
    {
        if (fishMaterial != null)
        {
            fishMaterial.color = color;
        }
    }
}
