using UnityEngine;

// Unity標準のアニメーション(Animator / Animation)を1回再生し終わったら、このオブジェクトを消す。
// エフェクトのプレハブに付けるだけで使える。
public class DestroyAfterAnimation : MonoBehaviour
{
    // 終わりの何割手前で消すか。0.02なら、再生の98%まで進んだ時点で消す。
    // (最後のコマまで見せたいなら小さく、1コマ目に戻って見えるなら大きくする)
    [SerializeField, Range(0.0f, 0.2f)] float END_MARGIN = 0.02f;

    Animator _animator;
    bool _destroyed;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // Animatorが無い場合(旧Animation)は、クリップの長さで消す
        if (_animator == null)
        {
            Animation legacy = GetComponentInChildren<Animation>();
            float length = (legacy != null && legacy.clip != null) ? legacy.clip.length : 1.0f;
            Destroy(gameObject, length * (1.0f - END_MARGIN));
        }
    }

    void Update()
    {
        if (_animator == null || _destroyed) return;

        // 時間ではなく、アニメーションの進み具合(0〜1)で判定する
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 1.0f - END_MARGIN)
        {
            _destroyed = true;
            Destroy(gameObject);
        }
    }
}