using UnityEngine;

public class HitDamageArea : MonoBehaviour
{
    int id = 0;
    int damage = 0;
    Vector2 vec;

    private bool hit = false;

    [SerializeField] float lifeTime = 0.1f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)vec * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (hit) return;

        if (col.TryGetComponent<IBurst>(out var cb))
        {
            if (cb.id != id)
            {
                hit = true;

                cb.Damage(damage, id);

                Destroy(this);
            }
        }
    }

    public void Init(int id, int damage, Vector2 vec)
    {
        this.id = id;
        this.damage = damage;
        this.vec = vec;
    }
}