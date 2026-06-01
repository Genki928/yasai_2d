using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

static class Winner
{
    static public string w_name = "";
    static public int w_id;
    static public Sprite sprite;

    public static void Reset()
    {
        w_name = "";
        w_id = 0;
        sprite = null;
    }
}
public class BattleManager : MonoBehaviour
{
    const int PLAYER_CNT = 2;

    [Header("◇キャラ生成")]
    [SerializeField] List<Character> characters = new(); 
    public Spawner[] spawn_point = new Spawner[PLAYER_CNT];
    GameObject[] player = new GameObject[PLAYER_CNT];
    CharBase[] datas = new CharBase[PLAYER_CNT];
    int[] pick_nums = { 0, 0 };

    [Header("◇GUI")]
    public GUI[] gui = new GUI[PLAYER_CNT];

    //Px用
    public GameObject[] player_obj = new GameObject[PLAYER_CNT];
    GameObject[] p_obj = new GameObject[PLAYER_CNT];

    void Awake()
    {

        Winner.Reset();
        Application.targetFrameRate = 30;
        CharBase.OnPlayerDies += Finish;
    }
    void Start()
    {

        for (int i = 0; i < PLAYER_CNT; i++)
        {
            // ポインター
            p_obj[i]=Instantiate(player_obj[i]);

            // プレイヤー生成
            pick_nums[i] = PlayerPick.pick[i];
            PlayerInput pi;

            if (i == 0)
            {
                pi = PlayerInput.Instantiate(
                     characters[pick_nums[i]].chars,
                    playerIndex: i,
                    controlScheme: "Controller2",
                    pairWithDevice: Gamepad.all[0]
                );
            }
            else
            {
                pi = PlayerInput.Instantiate(
                     characters[pick_nums[i]].chars,
                    playerIndex: i,
                    controlScheme: "Controller1",
                    pairWithDevice: Gamepad.all[1]
                );
            }

            // ★ここが正解
            pi.transform.position = spawn_point[i].point.transform.position;
            pi.transform.rotation = Quaternion.identity;

            // 識別IDを設定
            player[i] = pi.gameObject;
            datas[i] = player[i].GetComponent<CharBase>();
            datas[i].id = i;

            datas[i].direction = SetDirect(spawn_point[i].direct);

            // バーストバーとの紐づけ
            gui[i].bar.Init(player[i]);

            // 各種UIとの紐づけ
            if (player[i].TryGetComponent<CharBase>(out var p))
            {
                p.burst_bar = gui[i].bar;   // バースト
                gui[i].name.text = p.data.char_name;    // キャラ名
                p.cooltimer[0] = gui[i].skill1_cooltimer;   // スキル1のクールタイムを表示
                p.cooltimer[1] = gui[i].skill2_cooltimer;   // スキル2のクールタイムを表示
                gui[i].icon.sprite = characters[pick_nums[i]].icon; // アイコン
            }
        }
    }

    void Update()
    {
        // ポインター
        p_obj[0].transform.position = new(player[0].transform.position.x, player[0].transform.position.y+2.0f);
        p_obj[1].transform.position = new(player[1].transform.position.x, player[1].transform.position.y+2.0f);

    }

    /// <summary> バトルを終了させる </summary>
    /// <param name="id"> プレイヤーの識別id </param>
    void Finish(int id)
    {
        Debug.Log("Player " + id + " won!");

        Winner.w_id = id;
        Winner.w_name = datas[id].data.char_name;
        Debug.Log($"id = {id}");
        Debug.Log(player[id]);
        Debug.Log(datas[id]);
        Winner.sprite = player[id].GetComponent<SpriteRenderer>().sprite;
        Debug.Log(Winner.sprite);

        SceneManager.LoadScene("ResultScene");
    }

    Vector2 SetDirect(DIRECT direct)
    {
        if (direct == DIRECT.RIGHT) return new(1.0f, 0.0f);
        if (direct == DIRECT.LEFT) return new(-1.0f, 0.0f);
        if (direct == DIRECT.UP) return new(0.0f, 1.0f);
        if (direct == DIRECT.DOWN) return new(0.0f, -1.0f);
        return Vector2.zero;
    }
}

[Serializable]
public class GUI
{
    public BurstBar bar;
    public Text name;
    public SpriteRenderer icon;
    public SkillCooltimer skill1_cooltimer;
    public SkillCooltimer skill2_cooltimer;
}

[Serializable]
public class Character
{
    public GameObject chars;
    public Sprite icon;
}

[Serializable]
public class Spawner
{
    public GameObject point;
    public DIRECT direct;
}

public enum DIRECT
{
    RIGHT,
    LEFT,
    UP,
    DOWN
}