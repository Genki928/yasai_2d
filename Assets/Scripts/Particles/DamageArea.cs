using UnityEngine;

public class DamageArea : MonoBehaviour
{
    int id = 0;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<CharBase>(out var cb))
        {
            if(cb.id != id)
            {
                cb.Damage(100);
                Debug.Log("damage");
            }
        }
    }

    public void Init(int id)
    {
        this.id = id;
    }
}
