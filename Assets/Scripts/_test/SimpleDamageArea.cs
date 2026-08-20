using System.Collections.Generic;
using UnityEngine;

public class SimpleDamageArea : DamageAreaBase
{
    // ----- メンバ ----- //
    List<int> _hitId = new();
    bool _delete;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<IBurst>(out var cb))
        {
            // 中断処理
            int check = _hitId.Find(cId => cb.id == cId);
            if (check > 0 || _id == cb.id) return;

            // ダメージ
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
    public void Init(int id, Damage damage, Vector2 vec, float lifeTime = 0.0f, bool delete = false)
    {
        _id = id;
        _damage = damage;
        if (lifeTime > 0.0f) Destroy(gameObject, lifeTime);
        _delete = delete;

        // 物理
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.linearVelocity = vec;
    }
}