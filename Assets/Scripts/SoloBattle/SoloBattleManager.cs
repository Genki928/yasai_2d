using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//static class Winner
//{
//    static public string w_name = "";
//    static public int w_id;
//    static public Sprite sprite;

//    public static void Reset()
//    {
//        w_name = "";
//        w_id = 0;
//        sprite = null;
//    }
//}
public class SoloBattleManager : MonoBehaviour
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
        // ポインター
        p_obj[0] = Instantiate(player_obj[0]);

        // プレイヤー生成
        player[0] = Instantiate(characters[3].chars, spawn_point[0].point.transform.position, Quaternion.identity);

        // 識別IDを設定
        datas[0] = player[0].GetComponent<CharBase>();
        datas[0].id = 0;

        datas[0].direction = SetDirect(spawn_point[0].direct);

        // バーストバーとの紐づけ
        gui[0].bar.Init(player[0]);

        // 各種UIとの紐づけ
        if (player[0].TryGetComponent<CharBase>(out var p))
        {
            p.burst_bar = gui[0].bar;   // バースト
            gui[0].name.text = p.data.char_name;    // キャラ名
            p.cooltimer[0] = gui[0].skill1_cooltimer;   // スキル1のクールタイムを表示
            p.cooltimer[1] = gui[0].skill2_cooltimer;   // スキル2のクールタイムを表示
            gui[0].icon.sprite = characters[pick_nums[0]].icon; // アイコン
        }
    }
    

    void Update()
    {
        // ポインター
        p_obj[0].transform.position = new(player[0].transform.position.x, player[0].transform.position.y + 2.0f);
    }

    void OnDestroy()
    {
        CharBase.OnPlayerDies -= Finish;
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
        Winner.sprite = datas[id].GetDefaultImage();
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

