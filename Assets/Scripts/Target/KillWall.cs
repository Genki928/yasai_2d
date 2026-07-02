using UnityEngine;

public class DeadTarget : MonoBehaviour
{
    [SerializeField] TargetBase target;

    void Update()
    {
        if (target == null)
        {
            transform.Rotate(0, 0, 10);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("KillWall"))
        {
            Destroy(gameObject);
        }
    }
}
