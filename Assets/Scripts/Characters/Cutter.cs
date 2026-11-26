using UnityEngine;

/// <summary>
/// キャベツの小さいカッター(Skill2)
/// ・生成後、launchDelay 秒のあいだ持ち主の周りに留まる
/// ・発射の瞬間に「持ち主以外で一番近い CharBase」を探し、そこへ向かって一直線に飛ぶ
/// ・CharBase が見つからなければ、生成時の持ち主の向き(fallbackDir)へ飛ぶ
/// ・当たったら1ヒットで消える
/// プレハブには Collider2D(Is Trigger ON)を付けてください。
/// </summary>
public class CabbageCutter : MonoBehaviour
{
    [SerializeField] float spinSpeed = 1080f;   // 回転速度(度/秒)

    CharBase owner;
    int ownerId;
    Vector2 offset;         // 持ち主からの相対位置(留まっている間)
    Vector2 fallbackDir;    // 敵がいないときの飛ぶ向き
    int damage;
    float speed;
    float lifetime;
    float launchDelay;
    float searchRadius;

    float waitTimer = 0f;
    float flyTimer = 0f;
    bool launched = false;
    Vector2 velocity;

    /// <param name="owner"> 生成したキャラ </param>
    /// <param name="offset"> 持ち主からの相対位置 </param>
    /// <param name="fallbackDir"> 敵が見つからないときの飛ぶ向き </param>
    /// <param name="damage"> ダメージ </param>
    /// <param name="speed"> 飛ぶ速さ </param>
    /// <param name="lifetime"> 発射してから消えるまでの時間 </param>
    /// <param name="launchDelay"> 生成してから発射するまでの時間 </param>
    /// <param name="searchRadius"> 敵を探す範囲(0以下なら無制限) </param>
    public void Init(CharBase owner, Vector2 offset, Vector2 fallbackDir, int damage,
                     float speed, float lifetime, float launchDelay, float searchRadius)
    {
        this.owner = owner;
        ownerId = owner.id;
        this.offset = offset;
        this.fallbackDir = fallbackDir.normalized;
        this.damage = damage;
        this.speed = speed;
        this.lifetime = lifetime;
        this.launchDelay = launchDelay;
        this.searchRadius = searchRadius;
    }

    void Update()
    {
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        if (!launched)
        {
            // 持ち主の周りに留まる(持ち主が消えたらその場で待つ)
            if (owner != null)
                transform.position = owner.transform.position + (Vector3)offset;

            waitTimer += Time.deltaTime;
            if (waitTimer >= launchDelay) Launch();
            return;
        }

        // 一直線に飛ぶ
        transform.position += (Vector3)(velocity * Time.deltaTime);

        flyTimer += Time.deltaTime;
        if (flyTimer >= lifetime) Destroy(gameObject);
    }

    void Launch()
    {
        launched = true;

        // 敵がいなければ、生成時の向きに飛ぶ
        Vector2 dir = fallbackDir;

        CharBase target = FindNearestEnemy();
        if (target != null)
        {
            Vector2 to = (Vector2)target.transform.position - (Vector2)transform.position;
            if (to.sqrMagnitude > 0.0001f) dir = to.normalized;
        }

        velocity = dir * speed;
    }

    /// <summary> 持ち主以外で、一番近い CharBase を探す </summary>
    CharBase FindNearestEnemy()
    {
        CharBase nearest = null;
        float best = float.MaxValue;

        foreach (var c in FindObjectsByType<CharBase>(FindObjectsSortMode.None))
        {
            if (c == owner || c.id == ownerId) continue;
            if (!c.isActiveAndEnabled) continue;

            float sqr = ((Vector2)c.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (searchRadius > 0f && sqr > searchRadius * searchRadius) continue;

            if (sqr < best)
            {
                best = sqr;
                nearest = c;
            }
        }
        return nearest;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!launched) return;  // 留まっている間は当たらない

        var target = other.GetComponentInParent<CharBase>();
        if (target == null || target == owner || target.id == ownerId) return;

        target.Damage(new(damage, DamageType.Soundable), ownerId);
        Destroy(gameObject);
    }
}