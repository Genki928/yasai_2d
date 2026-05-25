using UnityEngine;

public class HitDamageArea : MonoBehaviour
{
    int id = 0;
    int damage = 0;
    Vector2 vec;

    // 生存時間
    [SerializeField] float lifeTime = 0.1f;

    void Start()
    {
        // 一定時間後に自動削除
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)vec * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // CharBaseを持っているか
        if (col.TryGetComponent<CharBase>(out var cb))
        {
            // 自分以外ならダメージ
            if (cb.id != id)
            {
                cb.Damage(damage);

                // 当たったら消す
                Destroy(gameObject);
            }
        }
    }

    // 初期化
    public void Init(int id, int damage, Vector2 vec)
    {
        this.id = id;
        this.damage = damage;
        this.vec = vec;
    }
}