using UnityEngine;

public class MushroomGass : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private float gravity = 8f;
    [SerializeField] private float upPower = 3f;

    [Header("サイズ変化")]
    [SerializeField] private float maxScale = 1.5f;

    [SerializeField] private GameObject gasPrefab;

    private int ownerId;
    private int damage;
    private Vector2 moveVec;

    private float timer;
    private Vector3 defaultScale;

    public void Init(int id, int damage, Vector2 dir)
    {
        ownerId = id;
        this.damage = damage;

        // 初速度
        moveVec = dir.normalized * speed;
        moveVec.y += upPower;

        defaultScale = transform.localScale;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // 重力
        moveVec.y -= gravity * Time.deltaTime;

        // 移動
        transform.position += (Vector3)(moveVec * Time.deltaTime);

        // 頂点で最大になるサイズ変化
        float t = timer / lifeTime;
        float scale = Mathf.Lerp(1f, maxScale, Mathf.Sin(t * Mathf.PI));

        transform.localScale = defaultScale * scale;
    }

    private void OnDestroy()
    {
        if (gasPrefab == null) return;

        GameObject obj = Instantiate(gasPrefab, transform.position, Quaternion.identity);

        obj.GetComponent<MushroomDamageArea>().Init(
            ownerId,
            damage
        );
    }
}