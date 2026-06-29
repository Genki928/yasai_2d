using System;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : MonoBehaviour, IBurst
{
    [Header("◇キャラクターデータ")]
    public int id { get; set; } = 100;
    public int burst { get; set; } = 0;
    public int max_burst { get; set; } = 100;
    [NonSerialized] public List<int> damaged = new();
    [NonSerialized] public int damage = 0;

    [Header("◇物理")]
    [NonSerialized] public Rigidbody2D rb;
    [SerializeField] protected Collider2D hit_box;

    [Header("◇フェーズ")]
    protected int state_cnt = 0;
    protected Dictionary<string, BossState> states = new();
    protected BossState state;


    public bool right = true;
    public List<GameObject> area_list = new();
    public Dictionary<string, GameObject> area_obj = new();
    public GameObject player;

    virtual protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Freeze(true);
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        state?.Update();
    }

    public void ChangeState(string phase_name)
    {
        state?.Exit();
        state = states[phase_name];
        state?.Enter(this);
    }

    public void Damage(int value, int id)
    {
        Debug.Log("damaged");
    }

    /// <summary> 硬直させる </summary>
    /// <param name="freeze"> 有効なら当たり判定を有効化、座標を固定 </param>
    public void Freeze(bool freeze)
    {
        if(freeze)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            hit_box.enabled = true;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.None;
            hit_box.enabled = false;
        }
    }

    public GameObject DisplayDamageArea(int list, Vector2 pos)
    {
        return Instantiate(area_list[list], pos, Quaternion.identity);
        
    }

    public void RemoveDamageArea(GameObject go)
    {
        Destroy(go);
    }
}

/// <summary> ボスニンジンの、タックル攻撃パターン </summary>
public class BossChill : BossState
{

    override public void Enter(BossBase bb)
    {
        ;
    }
    override public bool Update()
    {
        return false;
    }
    override public void Exit()
    {
        ;
    }
}
