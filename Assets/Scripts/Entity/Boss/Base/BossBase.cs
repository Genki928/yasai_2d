using System.Collections.Generic;
using UnityEngine;

public class BossBase : MonoBehaviour, IBurst
{
    [Header("◇キャラクターデータ")]
    public int id { get; set; } = 100;
    public int burst { get; set; } = 0;
    public int max_burst { get; set; } = 100;

    [Header("◇物理")]
    protected Rigidbody2D rb;
    [SerializeField] protected Collider2D hit_box;

    [Header("◇フェーズ")]
    protected int state_cnt = 0;
    protected BossState state;
    protected List<BossState> state_list = new();

    virtual protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Freeze(true);

        // フェーズ
        state = state_list[0];
        state?.Enter(this);
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        state?.Update(this);
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

    protected void EnterNextPhase()
    {
        state?.Exit(this);
        state = state_list[++state_cnt];
        state?.Enter(this);
    }
}