using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Const;

public class SoloBattleManager : MonoBehaviour
{
    // ----- 定数 -----
    const int SPAWN_COOLTIME = 60;

    // ----- 変数 -----

    [Header("◇キャラ生成")]
    [SerializeField] List<Character> characters = new(); 
    public Spawner player_spawn_point;
    GameObject player;
    int pick_nums = 0;

    [Header("◇的生成")]
    [SerializeField] List<TargetBase> targets = new();
    [SerializeField] List<Spawner> target_spawn_point = new();
    public int spawn_cooltime = 0;

    // -----
    public Timer timer;
    int score;

    [Header("◇GUI")]
    public GUI gui;

    void Awake()
    {

        Winner.Reset();
        Application.targetFrameRate = 30;
    }

    void Start()
    {
        // プレイヤー生成
        player = Instantiate(characters[3].chars, player_spawn_point.point.transform.position, Quaternion.identity);

        // バーストバーとの紐づけ
        gui.bar.Init(player);

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
        var target = Instantiate(targets[0]);
        target.sbm = this;
        timer.Init(5);
        timer.OnFinish += Finish;
    }
    

    void Update()
    {
        if (++spawn_cooltime > 60)
        {
            int spawn = UnityEngine.Random.Range(0, spawn_cooltime);
            Instantiate(targets[0], target_spawn_point[spawn].point.transform.position, Quaternion.identity);
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
        score += score + value < 0 ? -score : value;
    }
}