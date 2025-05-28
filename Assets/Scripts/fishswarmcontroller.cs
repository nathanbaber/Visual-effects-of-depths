using UnityEngine;

public class fishswarmcontroller : MonoBehaviour
{
    public Transform target; // Цель для косяка
    public float speed = 5f; // Скорость движения рыб
    public float rotationSpeed = 2f; // Скорость поворота рыб
    public float neighborDistance = 5f; // Радиус поиска ближайших соседей
    public float avoidanceWeight = 1f; // Вес избегания столкновений
    public float alignmentWeight = 1f; // Вес выравнивания направления
    public float cohesionWeight = 1f; // Вес притяжения к центру косяка
    public float targetWeight = 1f;  // Вес стремления к цели

    private ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];

        // Создание цели, если она не задана
        if (target == null)
        {
            GameObject targetObject = new GameObject("FlockTarget");
            target = targetObject.transform;
            target.position = transform.position; // Начальная позиция цели
        }

        InvokeRepeating("UpdateTargetPosition", 0f, 2f); // Обновляем позицию цели каждые 2 секунды
    }

    void Update()
    {
        int numParticlesAlive = particleSystem.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 velocity = particles[i].velocity;
            Vector3 position = particles[i].position;

            // Расчет векторов влияния
            Vector3 avoidance = CalculateAvoidance(i, numParticlesAlive);
            Vector3 alignment = CalculateAlignment(i, numParticlesAlive);
            Vector3 cohesion = CalculateCohesion(i, numParticlesAlive);
            Vector3 targetDirection = (target.position - position).normalized;

            // Применение весов
            avoidance *= avoidanceWeight;
            alignment *= alignmentWeight;
            cohesion *= cohesionWeight;
            targetDirection *= targetWeight;

            // Суммирование векторов влияния
            velocity += avoidance + alignment + cohesion + targetDirection * speed;

            // Ограничение скорости
            velocity = Vector3.ClampMagnitude(velocity, speed);

            // Обновление параметров частицы
            particles[i].velocity = velocity;

            //Поворот рыбы в направлении движения
            Quaternion rotation = Quaternion.LookRotation(velocity);
            particles[i].rotation3D = rotation.eulerAngles;
        }

        particleSystem.SetParticles(particles, numParticlesAlive);
    }


    // Расчет вектора избегания столкновений
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

    // Расчет вектора выравнивания направления
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

    // Расчет вектора притяжения к центру косяка
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


    // Функция для случайного изменения позиции цели
    void UpdateTargetPosition()
    {
        target.position = transform.position + Random.insideUnitSphere * 20f; // Случайная позиция вокруг косяка
    }
}