using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Wanted : MonoBehaviour
{
    // テキスト
    [SerializeField] Transform textTransform;
    [SerializeField] float moveSpeed = 2.0f;
    [SerializeField] float rotateTurns = 1f;      // 到着までに何回転するか(整数推奨)
    [SerializeField] float finalAngleZ = 45f;      // 最終的に止めたい角度
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
        // 進行度を0〜1にクランプ(超過しても最終位置・最終角度に固定される)
        float t = Mathf.Clamp01(time * moveSpeed);

        // 位置
        textTransform.position = Vector3.Lerp(textStartPos, myPos, t);

        // 回転: 開始角度から (回転数×360度 + 最終角度) まで t に比例させる
        float angle = Mathf.Lerp(0f, 360f * rotateTurns, t);
        textTransform.rotation = Quaternion.Euler(0f, 0f, finalAngleZ + angle);
    }
}
