using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] List<Sprite> sprites = new List<Sprite>();

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        spriteRenderer.sprite = Winner.sprite;

        Debug.Log("ƒLƒƒƒ‰"+sprites);
        //spriteRenderer.sprite = sprites[1];

    }
}