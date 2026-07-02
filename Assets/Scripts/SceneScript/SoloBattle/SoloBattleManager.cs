using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Const;
using UnityEngine.UI;

public class SoloBattleManager : MonoBehaviour
{
    // ----- 定数 -----
    const int SPAWN_COOLTIME = 60;
    const int BONUS_TIMELIMIT_FRAMERATE = 400;

    // ----- 変数 -----

    [Header("◇キャラ生成")]
    [SerializeField] List<Character> characters = new(); 
    public Spawner player_spawn_point;
    CharBase player;
    int pick_nums = 4;

    [Header("◇的生成")]
    [SerializeField] List<TargetBase> targets = new();
    [SerializeField] List<Spawner> target_spawn_point = new();
    [SerializeField] List<Sprite> target_sprites = new();
    public int spawn_cooltime = 0;

    // -----
    public Timer timer;
    int score;

    [Header("◇GUI")]
    [SerializeField] GUI gui;
    [SerializeField] Text score_text;
    [SerializeField] Text score_bonus;
    [SerializeField] Image score_circle;
    float default_score_bonus = 1.0f;
    float now_score_bonus = 1.0f;
    int bonus_timer = 0;

    [Header("◇BGM")]
    public AudioSource audioSource;


    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        // プレイヤー生成
        player = Instantiate(characters[4].chars, player_spawn_point.point.transform.position, Quaternion.identity).GetComponent<CharBase>();
        //player.GetComponent<CharBase>().state.speed.Add(new() { value = 100, time = 100 });
        // バーストバーとの紐づけ
        gui.bar.Init(player.gameObject);

        // 各種UIとの紐づけ
        if (player.TryGetComponent<CharBase>(out var p))
        {
            p.id = 0;   // id
            p.direction = SetDirect(player_spawn_point.direct);    // 方向
            p.burst_bar = gui.bar;   // バースト
            gui.name.text = p.data.char_name;    // キャラ名
            p.cooltimer[0] = gui.skill1_cooltimer;   // スキル1のクールタイムを表示
            p.cooltimer[1] = gui.skill2_cooltimer;   // スキル2のクールタイムを表示
            gui.icon.sprite = characters[pick_nums].icon; // アイコン
        }

        //boss = Instantiate(bosses[0], new(-10.0f, -5.0f), Quaternion.identity);
        //boss.player = player;
        timer.Init(60);
        timer.OnFinish += Finish;
    }
    

    void Update()
    {
        // スコアボーナスが初期状態じゃない（ボーナスが付与されている）なら、
        if (now_score_bonus != 1.0f)
        {
            // 時間制限の更新
            score_circle.fillAmount = 1 - (float)++bonus_timer / BONUS_TIMELIMIT_FRAMERATE;
            
            // 時間制限が終わったなら、
            if (score_circle.fillAmount == 0)
            {
                // タイマー変数や時間制限の初期化
                score_circle.fillAmount = 1;
                now_score_bonus = 1.0f;

                // UIの更新
                score_bonus.text = "x " + now_score_bonus.ToString("N1");
            }
        }
        if (++spawn_cooltime > SPAWN_COOLTIME)
        {
            // ランダムな場所からスポーン
            int spawn = Random.Range(0, target_spawn_point.Count);
            TargetBase tb = Instantiate(targets[0], target_spawn_point[spawn].point.transform.position, Quaternion.identity);

            // Spriteを調整
            tb.Init(this, player.GetComponent<CharBase>(), transform.position.x > tb.transform.position.x ? true : false);

            // 操作キャラクターと画像が被らないよう調整
            int img = pick_nums;
            do
            {
                img = Random.Range(0, target_sprites.Count);
            } while (img == pick_nums);
            tb.GetComponent<SpriteRenderer>().sprite = target_sprites[img];

            // 見やすく（黒く）する
            tb.GetComponent<SpriteRenderer>().color = new Color32(128, 128, 128, 255);

            // タイマーをリセット
            spawn_cooltime = 0;
        }
    }

    void OnDestroy()
    {
        ;
    }

    /// <summary> バトルを終了させる </summary>
    /// <param name="id"> プレイヤーの識別id </param>
    public void Finish()
    {
        SoloBattleResult.name = player.data.char_name;
        SoloBattleResult.socre = score;
        SoloBattleResult.img = player.GetDefaultImage();
        SceneManager.LoadScene(SceneName.RESULT_PVE);
    }

    Vector2 SetDirect(DIRECT direct)
    {
        if (direct == DIRECT.RIGHT) return new(1.0f, 0.0f);
        if (direct == DIRECT.LEFT) return new(-1.0f, 0.0f);
        if (direct == DIRECT.UP) return new(0.0f, 1.0f);
        if (direct == DIRECT.DOWN) return new(0.0f, -1.0f);
        return Vector2.zero;
    }

    public void CalculateScore(int value)
    {
        // 計算（0未満になるなら調整）
        score += score + value < 0 ? -score : Mathf.RoundToInt(value * now_score_bonus);
        score_text.text = score.ToString();
        score_circle.fillAmount = 1;
        now_score_bonus += 0.1f;
        score_bonus.text = "x " + now_score_bonus.ToString("N1");
        bonus_timer = 0;
    }
}

static public class SoloBattleResult
{
    static public string name = "オレ";
    static public int socre = 2000;
    static public Sprite img;
}