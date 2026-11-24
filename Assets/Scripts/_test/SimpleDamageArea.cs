
using System.Collections.Generic;
using UnityEngine;

public class SimpleDamageArea : DamageAreaBase
{
    // 命中済みのプレイヤーID
    HashSet<int> _hitId = new();

    bool _delete = false;
    float _rigid;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<IBurst>(out var cb))
        {
            // 自分自身・命中済みの相手は無視
            if (_id == cb.id || _hitId.Contains(cb.id)) return;

            // 命中済みIDに追加
            _hitId.Add(cb.id);

            // ダメージ
            cb.rigid += _rigid;
            cb.Damage(_damage, _id);

            // 削除設定が有効なら破棄
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
    public void Init(
        int id,
        Damage damage,
        Vector2 vec,
        float lifeTime = 0.0f,
        bool delete = false,
        float rigid = 0.0f)
    {
        _id = id;
        _damage = damage;

        if (lifeTime > 0.0f)
            Destroy(gameObject, lifeTime);

        _delete = delete;
        _rigid = rigid;

        // 物理
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.linearVelocity = vec;
    }
}
