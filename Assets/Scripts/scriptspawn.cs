using UnityEngine;

public class scriptspawn : MonoBehaviour
{
    public ParticleSystem bubbleSystem;
    public float spawnInterval = 0.1f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;

            var emitParams = new ParticleSystem.EmitParams();
            emitParams.startSize = Random.Range(0.05f, 0.2f);
            emitParams.startLifetime = Random.Range(1f, 3f);
            emitParams.startColor = new Color(0.8f, 0.9f, 1f, Random.Range(0.3f, 0.7f));

            bubbleSystem.Emit(emitParams, 1);
        }
    }
}
