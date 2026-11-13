using UnityEngine;
using UnityEngine.U2D;

public class iwa : MonoBehaviour
{
    SpriteRenderer sprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        sprite.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}
