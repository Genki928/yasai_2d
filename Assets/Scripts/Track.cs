using UnityEngine;

public class Track : MonoBehaviour
{
    float _startX = 20.0f;
    float _endX = -20.0f;
    float _moveSpeed = -3.0f;
    Rigidbody2D _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.linearVelocityX = _moveSpeed;
    }

    void Update()
    {
        if (transform.position.x < _endX)
        {
            Vector2 pos = new(_startX, transform.position.y);
            transform.position = pos;
        }
    }
}
