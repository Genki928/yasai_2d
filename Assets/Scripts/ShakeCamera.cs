using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    float _shakeTime = 0;
    float _size = 0;
    Vector3 _startPos;
    bool _isShaking = false;

    void Update()
    {
        if (_isShaking)
        {
            if (--_shakeTime > 0)
            {
                float w = Random.Range((int)_startPos.x + -_size, (int)_startPos.x + _size + 1) / 100.0f;
                float h = Random.Range((int)_startPos.x + -_size, (int)_startPos.x + _size + 1) / 100.0f;
                transform.position = new Vector3(w, h, -10);
            }
            else
            {
                transform.position = _startPos;
                _isShaking = false;
            }
        }
    }

    public void Init(float time, float size)
    {
        _shakeTime = time;
        _size = size;
        _startPos = transform.position;
        _isShaking = true;
    }
}
