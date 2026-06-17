using UnityEngine;

public class DamageArea : MonoBehaviour
{
    int id = 0;
    int damage = 0;
    Vector2 vec;
    bool delete = false;
    bool delete_script = false;

    void Update()
    {
        transform.position += (Vector3)vec;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // 接触したオブジェクトが、CharBaseを持っている（継承している）なら、
        if (col.TryGetComponent<IBurst>(out var cb))
        {
            // オブジェクトが持つ識別idが、攻撃主（自分が持つid）と異なれば、
            if (cb.id != id)
            {
                // 被弾処理
                cb.Damage(damage,id);

                // 削除処理
                if (delete)
                {
                    Destroy(gameObject);
                    return;
                }
                if (cb.burst >= cb.max_burst || delete_script) Destroy(this);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    /// <summary> 識別idの紐づけ </summary>
    /// <param name="id"> 攻撃id</param>
    /// <param name="damage"> ダメージ量 </param>
    /// <param name="vec"> 方向 </param>
    /// <param name="delete"> ヒット後の削除 </param>
    public void Init(int id, int damage, Vector2 vec, bool delete = false, bool script = false)
    {
        this.id = id;
        this.damage = damage;
        this.vec = vec;
        this.delete = delete;
        this.delete_script = script;
    }
}
