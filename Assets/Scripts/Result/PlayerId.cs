using System.Collections.Generic;
using UnityEngine;

public class PlayerId : MonoBehaviour
{
    [SerializeField] List<Sprite> sprites = new List<Sprite>();

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = sprites[Winner.w_id];
    }

    void Update()
    {
        spriteRenderer.sprite = sprites[Winner.w_id];
        //spriteRenderer.sprite = sprites[1];

    }
}