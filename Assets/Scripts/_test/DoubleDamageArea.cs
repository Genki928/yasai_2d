using UnityEngine;

public class DoubleDamageArea : DamageAreaBase
{
    // ----- メンバ ----- //
    bool _delete = false;
    float _rigid;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<IBurst>(out var cb))
        {
            // ダメージ
            cb.rigid += _rigid;
            cb.Damage(_damage, _id);
            if (_delete) Destroy(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    /// <summary> 初期化 </summary>
    /// <param name="id"> 攻撃者のID </param>
    /// <param name="damage"> 与えるダメージ </param>
    /// <param name="vec"> 移動させるベクトル </param>
    /// <param name="lifeTime"> 残留する時間 </param>
    /// <param name="delete"> 削除するかどうか </param>
    public void Init(int id, Damage damage, Vector2 vec, float lifeTime = 0.0f, bool delete = false, float rigid = 0.0f)
    {
        _id = id;
        _damage = damage;
        if (lifeTime > 0.0f) Destroy(gameObject, lifeTime);
        _delete = delete;
        _rigid = rigid;

        // 物理
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.linearVelocity = vec;
    }
}