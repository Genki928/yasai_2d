using UnityEngine;

public class Bomb : MonoBehaviour
{
    int id;
    public GameObject go;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<CharBase>(out var cb))
        {
            // オブジェクトが持つ識別idが、攻撃主（自分が持つid）と異なれば、
            if (cb.id != id)
            {
                cb.Damage(20, id);
                cb.rigid += 20;
                Destroy(gameObject);
                Instantiate(go, transform.position, Quaternion.identity);
            }
        }
    }

    public void Init(int id)
    {
        this.id = id;
    }
}
