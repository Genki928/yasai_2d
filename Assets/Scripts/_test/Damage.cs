using UnityEngine;

public class DamageAreaBase : MonoBehaviour
{
    // ----- メンバ ----- //
    protected int _id = 0;
    protected Damage _damage;
    protected Vector2 _vec = new(0.0f, 0.0f);
    protected Rigidbody2D _rigidbody;
}

public enum DamageType
{
    Soundable,
    Silentable
}

public class Damage
{
    // ----- メンバ ----- //
    public int Value => _value;
    public DamageType Type => _type;

    // ----- プロパティ ----- //
    int _value = 0;
    DamageType _type = DamageType.Silentable;

    public Damage(int value,  DamageType type)
    {
        _value = value;
        _type = type;
    }
}