using System.Collections;
using UnityEngine;

public class BambooSpear : MonoBehaviour
{
    [SerializeField] float SPEAR_SPEED = 0.08f;
    Vector3 direction;

    void Update()
    {
        transform.position += direction * SPEAR_SPEED;
    }

    public IEnumerator Spear(Vector3 dir)
    {
        direction = dir;
        yield return new WaitForSeconds(0.15f);
        direction = new(-dir.x, -dir.y, dir.z);
        yield return new WaitForSeconds(0.15f);
    }
}