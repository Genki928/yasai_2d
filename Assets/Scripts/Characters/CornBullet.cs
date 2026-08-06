using UnityEngine;

public class CornBullet : MonoBehaviour
{
    int id;
    public Sprite popcorn;
    Vector2 direction;
    GameObject bullet_obj;
    public GameObject bomb_obj;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Object"))
        {
            // 破棄される前に、bullet_objを親から切り離しておく
            bullet_obj.transform.SetParent(null);
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // 爆発生成
        GameObject particle = Instantiate(bomb_obj, bullet_obj.transform.position, Quaternion.identity);
        particle.GetComponent<HitDamageArea>().Init(id, 15, new(0, 0));

        //// ポップコーン生成
        //bullet_obj.GetComponent<SpriteRenderer>().sprite = popcorn;
        //bullet_obj.GetComponent<DamageArea>().Init(id, 0, new(0, 0), false);
        //Rigidbody2D rb_c = bullet_obj.GetComponent<Rigidbody2D>();
        //rb_c.gravityScale = 5.0f;
        //rb_c.linearVelocity = new(direction.x, 15.0f);
        //bullet_obj = null;
    }

    public void Init(int id, Vector2 dir, GameObject bullet)
    {
        this.id = id;
        this.direction = dir;
        this.bullet_obj = bullet;
    }
}