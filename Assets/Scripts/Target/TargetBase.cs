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
    bool right = false;
    [SerializeField] BombTarget bomb;
    [SerializeField] AudioClip damage;
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
        rb.linearVelocity = (player.transform.position - transform.position).normalized * 0.5f;
    }

    public void Damage(int value, int id)
    {
        // バースト値が最大なら中断
        if (burst >= max_burst) return;

        // 受けるダメージが過剰ならセーブする
        burst = burst + value > max_burst ?
                     max_burst : burst + value;

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

            audioSource.PlayOneShot(damage);
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

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<CharBase>(out var b))
        {
            //b.rigid += 60;
            //b.KnockBack(10, (col.transform.position - transform.position).normalized);
        }
    }

    void OnDestroy()
    {
        camera.Init(15, 5);
    }
}
