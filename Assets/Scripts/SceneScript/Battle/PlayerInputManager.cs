using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;
    private Rigidbody2D rb;
    [SerializeField] float speed = 5f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    public void OnJump()
    {
        Debug.Log($"{gameObject.name} Jump");
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * speed;
    }
}