using UnityEngine;

public class TargetBase : MonoBehaviour, IBurst
{
    public int id { get; set; } = 100;
    public int burst { get; set; } = 0;
    public int max_burst { get; set; } = 10;
    public int rigid { get; set; } = 0;
    Rigidbody2D rb;
    int score = 100;
    public SoloBattleManager sbm;
    public CharBase player;
    public bool right { get; private set; } = false;
    public bool escape = false;
    [SerializeField] BombTarget bomb;
    [SerializeField] AudioClip _damage;
    AudioSource audioSource;
    ShakeCamera camera;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        // 低確率で爆弾を持たせる
        if (UnityEngine.Random.Range(0, 10) == 0)
        {
            BombTarget bt = Instantiate(bomb, transform.position, Quaternion.identity);
            bt.Init(this);
            GetComponent<SpriteRenderer>().sortingOrder = -1;
        }
    }
    
    void Update()
    {
        ;
    }

    void FixedUpdate()
    {
        const float MoveSpeed = 3.0f;
        if (!escape)
            rb.linearVelocity = (player.transform.position - transform.position).normalized * 0.5f;
        else
        {
            if (right) rb.linearVelocity = new(-MoveSpeed, MoveSpeed);
            else rb.linearVelocity = new(MoveSpeed, MoveSpeed);
        }
    }

    public void Damage(Damage damage, int id)
    {
        if (sbm.gameset) return;

        // バースト値が最大なら中断
        if (burst >= max_burst) return;

        // 受けるダメージが過剰ならセーブする
        burst = burst + damage.Value > max_burst ?
                     max_burst : burst + damage.Value;

        // バースト値が最大なら、死亡
        if (burst == max_burst)
        {
            if (right)
            {
                rb.linearVelocity = new(-10, 10);
            }
            else
            {
                rb.linearVelocity = new(10, 10);
            }
            sbm.CalculateScore(score);

            audioSource.PlayOneShot(_damage);
            Destroy(this);
        }
    }

    public void Init(SoloBattleManager sbm, CharBase player, bool righrt, ShakeCamera camera)
    {
        this.sbm = sbm;
        this.player = player;
        this.right = righrt;
        this.camera = camera;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.TryGetComponent<CharBase>(out var b))
        {
            //b.rigid += 60;
            //b.KnockBack(10, (col.transform.position - transform.position).normalized);
            b.Damage(new(1, DamageType.Silentable), id);
        }
    }

    void OnDestroy()
    {
        camera.Init(15, 5);
    }
}
