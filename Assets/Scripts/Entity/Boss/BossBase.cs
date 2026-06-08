using UnityEngine;

public class BossBase : MonoBehaviour, IBurst
{

    [Header("◇キャラクターデータ")]
    public int id { get; set; } = 100;
    public int burst { get; set; } = 0;
    public int max_burst { get; set; } = 100;

    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    public void Damage(int value, int id)
    {
        Debug.Log("mi");
    }
}