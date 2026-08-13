using System.Collections.Generic;
using UnityEngine;

public class MushroomDamageArea : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float hitInterval = 0.5f;

    private int id;
    private int damage;

    // ç≈å„Ç…É_ÉÅÅ[ÉWÇó^Ç¶ÇΩéûä‘
    private Dictionary<int, float> hitTimer = new Dictionary<int, float>();

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Init(int id, int damage)
    {
        this.id = id;
        this.damage = damage;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!col.TryGetComponent<IBurst>(out var cb))
            return;

        if (cb.id == id)
            return;

        if (!hitTimer.ContainsKey(cb.id))
        {
            hitTimer.Add(cb.id, -hitInterval);
        }

        if (Time.time - hitTimer[cb.id] >= hitInterval)
        {
            if (col.TryGetComponent<CharBase>(out var b)) b.State[(int)StateName.Speed].AddState(new Attribute(-2.0f, 1.0f));
            cb.Damage(new(damage, DamageType.Silentable), id);
            hitTimer[cb.id] = Time.time;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.TryGetComponent<IBurst>(out var cb))
        {
            hitTimer.Remove(cb.id);
        }
    }
}