using UnityEngine;

public class CursorDirection : MonoBehaviour
{
    [SerializeField] float scaleSizeMax = 1.1f;
    [SerializeField] float scaleSizeMin = 0.9f;
    [SerializeField] float scalingSize = 0.5f;
    bool scale = true;

    void Start()
    {
        Application.targetFrameRate = 60;
        transform.localScale = transform.localScale;
        Debug.Log(transform.localScale);
    }

    void Update()
    {
        ScaleChange(Time.deltaTime);
    }

    public void ScaleChange(float deltaTime)
    {
        if (scale)
        {
            // サイズ変更
            transform.localScale *= scalingSize;
            transform.localScale = new(transform.localScale.x, transform.localScale.y, 1.0f);

            // 一定以上のサイズなら反転
            if (transform.localScale.magnitude > scaleSizeMax)
                scale = false;
        }
        else
        {
            // サイズ変更
            transform.localScale /= scalingSize;
            transform.localScale = new(transform.localScale.x, transform.localScale.y, 1.0f);

            // 一定以下のサイズなら反転
            if (transform.localScale.magnitude < scaleSizeMin)
                scale = true;
        }
        Debug.Log(transform.localScale.magnitude + " / " + scaleSizeMin);
    }
}
