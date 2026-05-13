using UnityEngine;

public class DamageArea : MonoBehaviour
{
    int id = 0;
    int damage = 0;
    Vector2 vec;

    void OnTriggerEnter2D(Collider2D col)
    {
        // 接触したオブジェクトが、CharBaseを持っている（継承している）なら、
        if (col.TryGetComponent<CharBase>(out var cb))
        {
            // オブジェクトが持つ識別idが、攻撃主（自分が持つid）と異なれば、
            if(cb.id != id)
            {
                // 被弾処理
                cb.Damage(damage);
                Debug.Log("damage");
            }
        }
    }

    // 識別idの紐づけ
    public void Init(int id, int damage, Vector2 vec)
    {
        this.id = id;
        this.damage = damage;
    }
}
