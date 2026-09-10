using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour
{
    [SerializeField] float alphaPower;
    [SerializeField] float maxAlpha;
    [SerializeField] float minAlpha;
    [SerializeField] float startDelay;

    // -----
    float delay = 0.0f;
    bool stop = true;
    float timer = 0.0f;
    bool reverse = false;
    Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        // 中断
        if (stop) return;
        if (delay < startDelay)
        {
            delay += Time.deltaTime;
            return;
        }

        // タイマーを進める
        if (!reverse) timer = Mathf.Min(1.0f, timer + Time.deltaTime);
        else timer = Mathf.Max(0.0f, timer - Time.deltaTime);
        float a = timer * alphaPower;

        // 反転
        if (a >= maxAlpha) reverse = true;
        if (a <= minAlpha) reverse = false;

        // 色を設定
        image.color = new(1.0f, 0.0f, 0.0f, a);
    }

    public void StartFlash()
    {
        stop = false;
    }
}
