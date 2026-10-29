using UnityEngine;

public class ReadyText : MonoBehaviour
{
    float time = 0.0f;

    // テキスト
    [SerializeField] Transform textTransform;
    [SerializeField] float moveSpeed = 2.0f;
    public Vector3 textStartPos;
    Vector3 myPos;

    // 背景
    [SerializeField] Transform backTransform;

    void OnEnable()
    {
        myPos = transform.position;
        textStartPos = textTransform.position;
    }

    void OnDisable()
    {
        time = 0.0f;
        textTransform.position = textStartPos;
    }

    void Update()
    {
        time += Time.deltaTime;
        Move();
        Scaling();
    }

    bool Move()
    {
        textTransform.position = Vector3.Lerp(textStartPos, myPos, time * moveSpeed);
        return Vector3.Distance(textTransform.position, myPos) < 0.01;
    }

    bool Scaling()
    {
        backTransform.localScale = Vector3.Lerp(new(1.0f, 0.0f, 1.0f), new(1.0f, 1.0f, 1.0f), time * moveSpeed);
        return Vector3.Distance(backTransform.localScale, new(1.0f, 1.0f, 1.0f)) < 0.01;
    }
}