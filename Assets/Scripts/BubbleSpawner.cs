using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    public GameObject bubblePrefab;
    public Vector2 bubbleSizeRange = new Vector2(0.1f, 0.5f);
    public float spawnInterval = 1f;
    public float minVelocity = 0.5f;
    public float maxVelocity = 1.5f;
    public Vector3 spawnAreaMin = new Vector3(-5f, 0f, -5f);
    public Vector3 spawnAreaMax = new Vector3(5f, 0f, 5f);

    private float timer;

    private List<GameObject> bubbles = new List<GameObject>();

    public GameObject CreateBubble(Vector3 position)
    {
        GameObject bubble = Instantiate(bubblePrefab, position, Quaternion.identity);
        float size = Random.Range(bubbleSizeRange.x, bubbleSizeRange.y);
        bubble.transform.localScale = new Vector3(size, size, size);
        return bubble;
    }

    private IEnumerator FadeInBubble(Material mat, float duration)
    {
        float elapsed = 0f;
        Color color = mat.color;
        color.a = 0f;
        mat.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            mat.color = color;
            yield return null;
        }
        color.a = 1f;
        mat.color = color;
    }

    void UpdateBubble(GameObject bubble)
    {
        float speed = Random.Range(minVelocity, maxVelocity);
        bubble.transform.position += Vector3.up * speed * Time.deltaTime;
    }

    private IEnumerator PopBubble(GameObject bubble, Material mat, float duration)
    {
        float elapsed = 0f;
        Vector3 originalScale = bubble.transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            bubble.transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.5f, t);

            Color color = mat.color;
            color.a = Mathf.Lerp(1f, 0f, t);
            mat.color = color;

            yield return null;
        }
        Destroy(bubble);
    }

    void SpawnBubble()  // ћетод дл€ вызова из Update, чтобы создавать одну пузырьку
    {
        Vector3 position = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            spawnAreaMin.y,
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
        );
        GameObject bubble = CreateBubble(position);
        Material mat = bubble.GetComponent<Renderer>().material;
        StartCoroutine(FadeInBubble(mat, 0.5f));
        bubbles.Add(bubble);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnBubble();
        }

        // ћожно тут обновл€ть положение пузырей
        for (int i = bubbles.Count - 1; i >= 0; i--)
        {
            GameObject bubble = bubbles[i];
            if (bubble != null)
            {
                UpdateBubble(bubble);
                // ≈сли пузырь вышел за пределы, можно запустить Pop и удалить из списка
                if (bubble.transform.position.y > 10f) // примерна€ высота выхода
                {
                    Material mat = bubble.GetComponent<Renderer>().material;
                    StartCoroutine(PopBubble(bubble, mat, 0.5f));
                    bubbles.RemoveAt(i);
                }
            }
            else
            {
                bubbles.RemoveAt(i);
            }
        }
    }
}