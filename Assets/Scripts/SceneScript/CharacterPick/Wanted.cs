using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Wanted : MonoBehaviour
{
    // テキスト
    [SerializeField] Transform textTransform;
    [SerializeField] float moveSpeed = 2.0f;
    public Vector3 textStartPos;
    Vector3 myPos;
    float time = 0.0f;

    void Start()
    {
        myPos = transform.position;
        textStartPos = textTransform.position;
    }

    void Update()
    {
        time += Time.deltaTime;
        Move();
    }

    void Move()
    {
        textTransform.position = Vector3.Lerp(textStartPos, myPos, time * moveSpeed);
        if (Vector3.Distance(textTransform.position, transform.position) > 0.01f)
            textTransform.Rotate(0.0f, 0.0f, 15.0f);
    }
}
