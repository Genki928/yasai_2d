using UnityEngine;

public class LeekGhostFade : MonoBehaviour
{
    SpriteRenderer sr;
    float lifeTime;
    float timer;
    void Awake()
    {
        //sr = GetComponent<SpriteRenderer>();
    }
    public void Initialize(float time)
    {
        sr = GetComponent<SpriteRenderer>();
        lifeTime = time;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        timer += Time.deltaTime;

        Color color = sr.color;
        color.a = 1f - timer / lifeTime;
        sr.color = color;

        //if (timer >= lifeTime)
        //{
        //    Destroy(gameObject);
        //}
    }
}