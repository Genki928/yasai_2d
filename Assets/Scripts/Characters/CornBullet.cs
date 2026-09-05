using UnityEngine;

public class CornBullet : MonoBehaviour
{
    int id;
    public GameObject popcorn;
    public GameObject bomb_obj;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<CharBase>(out var cb) && cb.id != id)
            Bomb();
        if (collision.CompareTag("Wall"))
            Bomb();

    }

    public void Init(int id, Vector2 dir, GameObject bullet)
    {
        this.id = id;
    }

    void Bomb()
    {

        // îöî≠ê∂ê¨
        Instantiate(popcorn, transform.position, Quaternion.identity);
        GameObject particle = Instantiate(bomb_obj, transform.position, Quaternion.identity);
        particle.GetComponent<SimpleDamageArea>().Init(id, new(15, DamageType.Soundable), new(0, 0));
        Destroy(gameObject);
    }
}