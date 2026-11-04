using UnityEngine;

public class LeekAfterImage : MonoBehaviour
{
    [SerializeField] float interval = 0.025f;
    [SerializeField] float lifeTime = 0.2f;

    float timer;
    SpriteRenderer sr;

    public bool isActive;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0;
            CreateAfterImage();
        }
    }

    void CreateAfterImage()
    {
        GameObject ghost = new GameObject("LeekAfterImage");

        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;
        ghost.transform.localScale = transform.lossyScale;

        SpriteRenderer ghostSr = ghost.AddComponent<SpriteRenderer>();

        ghostSr.sprite = sr.sprite;
        ghostSr.flipX = sr.flipX;
        ghostSr.flipY = sr.flipY;
        ghostSr.color = sr.color;
        ghostSr.sortingLayerID = sr.sortingLayerID;
        ghostSr.sortingOrder = sr.sortingOrder - 1;

        LeekGhostFade fade = ghost.AddComponent<LeekGhostFade>();
        fade.Initialize(lifeTime);
    }
}