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
    [Header("◇キャラ生成")]
    [SerializeField] List<Character> characters = new(); 
    public Spawner spawn_point;
    GameObject player;
    CharBase datas;
    int pick_nums = 0;

    [Header("◇GUI")]
    public GUI gui;

    //Px用
    public GameObject player_obj;
    GameObject p_obj;

    void Awake()
    {

        Winner.Reset();
        Application.targetFrameRate = 30;
        CharBase.OnPlayerDies += Finish;
    }

    void Start()
    {
        // ポインター
        p_obj = Instantiate(player_obj);

        // プレイヤー生成
        player = Instantiate(characters[3].chars, spawn_point.point.transform.position, Quaternion.identity);

        // 識別IDを設定
        datas = player.GetComponent<CharBase>();
        datas.id = 0;

        datas.direction = SetDirect(spawn_point.direct);

        // バーストバーとの紐づけ
        gui.bar.Init(player);

        // 各種UIとの紐づけ
        if (player.TryGetComponent<CharBase>(out var p))
        {
            p.burst_bar = gui.bar;   // バースト
            gui.name.text = p.data.char_name;    // キャラ名
            p.cooltimer[0] = gui.skill1_cooltimer;   // スキル1のクールタイムを表示
            p.cooltimer[1] = gui.skill2_cooltimer;   // スキル2のクールタイムを表示
            gui.icon.sprite = characters[pick_nums].icon; // アイコン
        }
    }
    

    void Update()
    {
        // ポインター
        p_obj.transform.position = new(player.transform.position.x, player.transform.position.y + 2.0f);
    }

    void OnDestroy()
    {
        CharBase.OnPlayerDies -= Finish;
    }

    /// <summary> バトルを終了させる </summary>
    /// <param name="id"> プレイヤーの識別id </param>
    void Finish(int id)
    {
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

