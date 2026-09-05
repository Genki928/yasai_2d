using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    int _id;
    Cooltime _cooltime;
    [SerializeField] SimpleDamageArea bomb;
    [SerializeField] SpriteRenderer _circleSpriteRenderer;
    [SerializeField] List<Sprite> _circleSprite = new();

    void Start()
    {
        ;
    }

    void Update()
    {
        ;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<IBurst>(out var cb))
        {
            // オブジェクトが持つ識別idが、攻撃主（自分が持つid）と異なれば、
            if (cb.id != _id)
            {
                // ダメージ
                _cooltime.RefleshCooltime(0.4f);
                cb.rigid += 0.5f;

                // 爆発
                var damage = Instantiate(bomb, transform.position, Quaternion.identity);
                damage.Init(_id ,new(50, DamageType.Soundable), new(0.0f, 0.0f));
                Destroy(gameObject);
            }
        }
    }

    public void Init(int id, Cooltime cooltime)
    {
        // 初期設定
        _id = id;
        _cooltime = cooltime;

        // 画像切り替え
        _circleSpriteRenderer.sprite = _circleSprite[id];
    }
}
