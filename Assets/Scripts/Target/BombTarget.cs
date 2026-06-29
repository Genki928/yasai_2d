
using UnityEngine;

public class BombTarget : MonoBehaviour
{
    GameObject target;
    [SerializeField] GameObject bomb;
    bool right = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (transform.position.x <= 0) right = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            if (right)
                transform.position = new(target.transform.position.x + 0.5f, target.transform.position.y + 0.5f);
            else
                transform.position = new(target.transform.position.x - 0.5f, target.transform.position.y + 0.5f);
        }
        else
        {
            GameObject paritcle = Instantiate(bomb, transform.position, Quaternion.identity);
            paritcle.GetComponent<DamageArea>().Init(1, 50, new(0, 0), false);
            Destroy(gameObject);
        }

    }

    public void Init(GameObject target)
    {
        this.target = target;
    }
}
