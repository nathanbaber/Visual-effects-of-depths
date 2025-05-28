using UnityEngine;

public class fishswarmcontroller : MonoBehaviour
{
    public Transform target; // ���� ��� ������
    public float speed = 5f; // �������� �������� ���
    public float rotationSpeed = 2f; // �������� �������� ���
    public float neighborDistance = 5f; // ������ ������ ��������� �������
    public float avoidanceWeight = 1f; // ��� ��������� ������������
    public float alignmentWeight = 1f; // ��� ������������ �����������
    public float cohesionWeight = 1f; // ��� ���������� � ������ ������
    public float targetWeight = 1f;  // ��� ���������� � ����

    private ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];

        // �������� ����, ���� ��� �� ������
        if (target == null)
        {
            GameObject targetObject = new GameObject("FlockTarget");
            target = targetObject.transform;
            target.position = transform.position; // ��������� ������� ����
        }

        InvokeRepeating("UpdateTargetPosition", 0f, 2f); // ��������� ������� ���� ������ 2 �������
    }

    void Update()
    {
        int numParticlesAlive = particleSystem.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 velocity = particles[i].velocity;
            Vector3 position = particles[i].position;

            // ������ �������� �������
            Vector3 avoidance = CalculateAvoidance(i, numParticlesAlive);
            Vector3 alignment = CalculateAlignment(i, numParticlesAlive);
            Vector3 cohesion = CalculateCohesion(i, numParticlesAlive);
            Vector3 targetDirection = (target.position - position).normalized;

            // ���������� �����
            avoidance *= avoidanceWeight;
            alignment *= alignmentWeight;
            cohesion *= cohesionWeight;
            targetDirection *= targetWeight;

            // ������������ �������� �������
            velocity += avoidance + alignment + cohesion + targetDirection * speed;

            // ����������� ��������
            velocity = Vector3.ClampMagnitude(velocity, speed);

            // ���������� ���������� �������
            particles[i].velocity = velocity;

            //������� ���� � ����������� ��������
            Quaternion rotation = Quaternion.LookRotation(velocity);
            particles[i].rotation3D = rotation.eulerAngles;
        }

        particleSystem.SetParticles(particles, numParticlesAlive);
    }


    // ������ ������� ��������� ������������
    private Vector3 CalculateAvoidance(int index, int numParticlesAlive)
    {
        Vector3 avoidance = Vector3.zero;
        int neighborCount = 0;

        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (i != index)
            {
                float distance = Vector3.Distance(particles[index].position, particles[i].position);
                if (distance < neighborDistance)
                {
                    avoidance += (particles[index].position - particles[i].position).normalized / distance;
                    neighborCount++;
                }
            }
        }

        if (neighborCount > 0)
        {
            avoidance /= neighborCount;
        }

        return avoidance;
    }

    // ������ ������� ������������ �����������
    private Vector3 CalculateAlignment(int index, int numParticlesAlive)
    {
        Vector3 alignment = Vector3.zero;
        int neighborCount = 0;

        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (i != index)
            {
                float distance = Vector3.Distance(particles[index].position, particles[i].position);
                if (distance < neighborDistance)
                {
                    alignment += particles[i].velocity;
                    neighborCount++;
                }
            }
        }

        if (neighborCount > 0)
        {
            alignment /= neighborCount;
            alignment = alignment.normalized;
        }

        return alignment;
    }

    // ������ ������� ���������� � ������ ������
    private Vector3 CalculateCohesion(int index, int numParticlesAlive)
    {
        Vector3 cohesion = Vector3.zero;
        int neighborCount = 0;

        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (i != index)
            {
                float distance = Vector3.Distance(particles[index].position, particles[i].position);
                if (distance < neighborDistance)
                {
                    cohesion += particles[i].position;
                    neighborCount++;
                }
            }
        }

        if (neighborCount > 0)
        {
            cohesion /= neighborCount;
            cohesion = (cohesion - particles[index].position).normalized;
        }

        return cohesion;
    }


    // ������� ��� ���������� ��������� ������� ����
    void UpdateTargetPosition()
    {
        target.position = transform.position + Random.insideUnitSphere * 20f; // ��������� ������� ������ ������
    }
}