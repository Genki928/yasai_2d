using UnityEngine;

public class Pop : MonoBehaviour
{
    const float X_MOVE = 5.0f;
    const float ROTATE_ANGLE = 5.0f;
    Rigidbody2D _rigidbody;
    bool _right;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.linearVelocity = new(Random.Range(-X_MOVE, X_MOVE), 10.0f);

        _right = _rigidbody.linearVelocityX > 0.0f;

    }

    void Update()
    {

        transform.Rotate(0.0f, 0.0f, _right ? ROTATE_ANGLE : -ROTATE_ANGLE);
    }
}
