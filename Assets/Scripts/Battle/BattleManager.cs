using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class BattleManager : MonoBehaviour
{
    [Header("◇キャラ生成")]
    [SerializeField] List<Character> characters = new(); 
    public GameObject[] spawn_point = new GameObject[2];
    GameObject[] player = new GameObject[2];
    CharBase[] datas = new CharBase[2];
    int[] pick_nums = { 1, 0 };

    [Header("◇GUI")]
    public GUI[] gui = new GUI[2];

    //Px用
    public GameObject[] player_obj = new GameObject[2];
    GameObject[] p_obj = new GameObject[2];

    void Awake()
    {
        Application.targetFrameRate = 30;
        CharBase.OnPlayerDies += Finish;
    }

    //void Start()
    //{
    //    for (int i = 0; i < 2; i++)
    //    {
    //        // キャラクターを生成
    //        player[i] = Instantiate(characters[pick_nums[i]].chars, spawn_point[i].transform.position, Quaternion.identity);
    //        datas[i] = player[i].GetComponent<CharBase>();
    //        datas[i].id = i;

    //        // ポインター
    //        p_obj[i]=Instantiate(player_obj[i]);

    //        // GUI
    //        gui[i].bar.Init(player[i]);
    //        if (player[i].TryGetComponent<CharBase>(out var p))
    //        {
    //            p.burst_bar = gui[i].bar;   // バーストゲージの表示
    //            gui[i].name.text = p.data.char_name;    // 名前の表示
    //            p.cooltimer[0] = gui[i].skill1_cooltimer;   // スキル1のクールタイム表示
    //            p.cooltimer[1] = gui[i].skill2_cooltimer;   // スキル2のクールタイム表示
    //            gui[i].icon.sprite = characters[pick_nums[i]].icon; // アイコン表示
    //        }
    //    }
    //}

    void Start()
    {

        for (int i = 0; i < 2; i++)
        {
            // ポインター
            p_obj[i]=Instantiate(player_obj[i]);

            GameObject prefab = characters[pick_nums[i]].chars;

            PlayerInput pi;

            if (i == 0)
            {
                pi = PlayerInput.Instantiate(
                    prefab,
                    playerIndex: i,
                    controlScheme: "Controller1",
                    pairWithDevice: Gamepad.all[0]
                );
            }
            else
            {
                pi = PlayerInput.Instantiate(
                    prefab,
                    playerIndex: i,
                    controlScheme: "Controller2",
                    pairWithDevice: Gamepad.all[1]
                );
            }

            // ★ここが正解
            pi.transform.position = spawn_point[i].transform.position;
            pi.transform.rotation = Quaternion.identity;

            player[i] = pi.gameObject;
            datas[i] = player[i].GetComponent<CharBase>();
            datas[i].id = i;

            gui[i].bar.Init(player[i]);

            if (player[i].TryGetComponent<CharBase>(out var p))
            {
                p.burst_bar = gui[i].bar;
                gui[i].name.text = p.data.char_name;
                p.cooltimer[0] = gui[i].skill1_cooltimer;
                p.cooltimer[1] = gui[i].skill2_cooltimer;
                gui[i].icon.sprite = characters[pick_nums[i]].icon;
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