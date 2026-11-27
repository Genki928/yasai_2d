using UnityEngine;

/// <summary>
/// 画面演出の共通ヘルパー(画面揺れ / ヒットストップ / ズームパンチ)
/// どのスクリプトからでも ScreenFx.Shake(...) のように呼べます。
/// シーンに置く必要はなく、初回呼び出し時に自動で生成されます。
/// カメラは Camera.main を対象にし、LateUpdate でオフセットを足し引きするので、
/// BattleManager のカメラ追従とは干渉しません。
/// </summary>
public class ScreenFx : MonoBehaviour
{
    static ScreenFx instance;
    static ScreenFx Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("ScreenFx");
                instance = go.AddComponent<ScreenFx>();
            }
            return instance;
        }
    }

    // ----- 公開API ----- //

    /// <summary> 画面を揺らす </summary>
    /// <param name="intensity"> 揺れの大きさ(ワールド単位。0.1〜0.5くらい) </param>
    /// <param name="duration"> 揺れる時間(秒) </param>
    public static void Shake(float intensity, float duration) => Instance.DoShake(intensity, duration);

    /// <summary> 一瞬だけ時間を止める(ヒットストップ) </summary>
    /// <param name="duration"> 止める時間(実時間の秒。0.05〜0.2くらい) </param>
    /// <param name="timeScale"> 止めている間の時間の流れ(0で完全停止) </param>
    public static void HitStop(float duration, float timeScale = 0.02f) => Instance.DoHitStop(duration, timeScale);

    /// <summary> 一瞬ズームして戻る </summary>
    /// <param name="amount"> マイナスでズームイン / プラスでズームアウト(カメラのサイズ差) </param>
    /// <param name="duration"> 戻るまでの時間(秒) </param>
    public static void ZoomPunch(float amount, float duration) => Instance.DoZoom(amount, duration);

    // ----- 内部 ----- //

    // 画面揺れ
    float shakeIntensity, shakeDuration, shakeElapsed = 1f;
    // ズーム
    float zoomAmount, zoomDuration, zoomElapsed = 1f;
    // 今フレームに足しているオフセット
    Vector3 lastPosOffset;
    float lastSizeOffset;
    // ヒットストップ
    bool hitStopping;
    float hitStopRemaining;
    float savedTimeScale = 1f;

    void DoShake(float intensity, float duration)
    {
        // 今の揺れより弱いものでは上書きしない
        float current = shakeElapsed < shakeDuration
            ? shakeIntensity * (1f - shakeElapsed / shakeDuration) : 0f;
        if (intensity < current) return;

        shakeIntensity = intensity;
        shakeDuration = Mathf.Max(0.01f, duration);
        shakeElapsed = 0f;
    }

    void DoZoom(float amount, float duration)
    {
        zoomAmount = amount;
        zoomDuration = Mathf.Max(0.01f, duration);
        zoomElapsed = 0f;
    }

    void DoHitStop(float duration, float timeScale)
    {
        if (!hitStopping)
        {
            savedTimeScale = Time.timeScale;
            hitStopping = true;
        }
        hitStopRemaining = Mathf.Max(hitStopRemaining, duration);
        Time.timeScale = timeScale;
    }

    void LateUpdate()
    {
        float dt = Time.unscaledDeltaTime;

        // ヒットストップの解除(実時間で数える)
        if (hitStopping)
        {
            hitStopRemaining -= dt;
            if (hitStopRemaining <= 0f) EndHitStop();
        }

        Camera cam = Camera.main;
        if (cam == null) return;

        // 前フレームに足したオフセットを外す(カメラ追従側の値に戻す)
        cam.transform.position -= lastPosOffset;
        if (cam.orthographic) cam.orthographicSize -= lastSizeOffset;
        lastPosOffset = Vector3.zero;
        lastSizeOffset = 0f;

        // 画面揺れ(だんだん弱くなる)
        if (shakeElapsed < shakeDuration)
        {
            shakeElapsed += dt;
            float falloff = Mathf.Max(0f, 1f - shakeElapsed / shakeDuration);
            Vector2 r = Random.insideUnitCircle * shakeIntensity * falloff;
            lastPosOffset = new Vector3(r.x, r.y, 0f);
        }

        // ズームパンチ(ぐっと寄ってゆっくり戻る)
        if (zoomElapsed < zoomDuration && cam.orthographic)
        {
            zoomElapsed += dt;
            float p = Mathf.Clamp01(zoomElapsed / zoomDuration);
            lastSizeOffset = zoomAmount * (1f - p) * (1f - p);
            // サイズが0以下にならないようにする
            lastSizeOffset = Mathf.Max(lastSizeOffset, 0.5f - cam.orthographicSize);
        }

        cam.transform.position += lastPosOffset;
        if (cam.orthographic) cam.orthographicSize += lastSizeOffset;
    }

    void EndHitStop()
    {
        hitStopping = false;
        hitStopRemaining = 0f;
        Time.timeScale = savedTimeScale;
    }

    void OnDestroy()
    {
        // ヒットストップ中にシーンが切り替わっても、時間が止まったままにならないようにする
        if (hitStopping) Time.timeScale = savedTimeScale;
        if (instance == this) instance = null;
    }
}