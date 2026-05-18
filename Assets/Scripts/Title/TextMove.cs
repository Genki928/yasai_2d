using UnityEngine;

public class TextMove : MonoBehaviour
{
    RectTransform rect;

    [SerializeField] float speed = 5f;

    // 開始位置
    [SerializeField]Vector2 startPos = new Vector2(-1000, 250);

    // 目的位置
    [SerializeField]Vector2 targetPos = new Vector2(0,250);

    void Start()
    {
        rect = GetComponent<RectTransform>();
        rect.anchoredPosition = startPos;
    }

    void Update()
    {
        rect.anchoredPosition = Vector2.Lerp(
            rect.anchoredPosition,
            targetPos,
            speed * Time.deltaTime
        );
        if (Vector2.Distance(rect.anchoredPosition, targetPos) < 1f)
        {
            rect.anchoredPosition = targetPos;
        }
    }
}