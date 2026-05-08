using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] List<GameObject> chars = new List<GameObject>();
    public GameObject[] spawn_point = new GameObject[2];
    GameObject[] player = new GameObject[2];
    int[] pick_nums = { 1, 0 };

    void Awake()
    {
        CharBase.OnPlayerDies += Finish;
    }

    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            // キャラクターを生成
            Vector2 pos = new Vector2(spawn_point[i].transform.position.x, spawn_point[i].transform.position.y);
            player[i] = Instantiate(chars[pick_nums[i]], pos, Quaternion.identity);
        }
    }

    /// <summary> バトルを終了させる </summary>
    /// <param name="id"> プレイヤーの識別id </param>
    void Finish(int id)
    {
        Debug.Log("Player " + id + " won!");
    }
}