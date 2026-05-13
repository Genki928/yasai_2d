using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] List<GameObject> chars = new();
    public GameObject[] spawn_point = new GameObject[2];
    GameObject[] player = new GameObject[2];
    int[] pick_nums = { 1, 0 };

    [Header("◇GUI")]
    [SerializeField] GameObject[] bars = new GameObject[2];
    [SerializeField] GameObject[] names = new GameObject[2];

    void Awake()
    {
        Application.targetFrameRate = 30;
        CharBase.OnPlayerDies += Finish;
    }

    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            // キャラクターを生成
            Vector2 pos = new(spawn_point[i].transform.position.x, spawn_point[i].transform.position.y);
            player[i] = Instantiate(chars[pick_nums[i]], pos, Quaternion.identity);
            player[i].GetComponent<CharBase>().id = i;

            // UI
            if(bars[i].TryGetComponent<BurstBar>(out var bar)) bar.Init(player[i]);
            if (names[i].TryGetComponent<Text>(out var text)) text.text = player[i].GetComponent<CharBase>().data.char_name;
        }
    }

    /// <summary> バトルを終了させる </summary>
    /// <param name="id"> プレイヤーの識別id </param>
    void Finish(int id)
    {
        Debug.Log("Player " + id + " won!");
    }
}