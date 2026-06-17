using UnityEngine;

public class AfterImage : MonoBehaviour
{
    [SerializeField] float interval = 0.05f; // 残像生成間隔
    [SerializeField] float lifeTime = 0.3f;  // 残像の寿命

    float timer;
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0;

            // 残像オブジェクト生成
            GameObject ghost = new GameObject("AfterImage");

            ghost.transform.position = transform.position;
            ghost.transform.rotation = transform.rotation;
            ghost.transform.localScale = transform.localScale;

            // SpriteRenderer追加
            SpriteRenderer ghostSr = ghost.AddComponent<SpriteRenderer>();
            ghostSr.sprite = sr.sprite;
            ghostSr.flipX = sr.flipX;
            ghostSr.flipY = sr.flipY;
            ghostSr.color = sr.color;
            ghostSr.sortingLayerID = sr.sortingLayerID;
            ghostSr.sortingOrder = sr.sortingOrder - 1;

            // 自動で消えるスクリプトを追加
            ghost.AddComponent<GhostFade>().Initialize(lifeTime);
        }
    }
}

public class GhostFade : MonoBehaviour
{
    float lifeTime;
    float timer;
    SpriteRenderer sr;

    public void Initialize(float time)
    {
        lifeTime = time;
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        Color color = sr.color;
        color.a = Mathf.Lerp(1f, 0f, timer / lifeTime);
        sr.color = color;

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}