using System.Collections.Generic;
using UnityEngine;

public class State
{
    // ----- プロパティ ----- //
    public float Generic => _generic;
    public List<Attribute> Buff => _buff;
    public List<Attribute> Debuff => _debuff;
    public float CurrentState => _currentState;

    // ----- メンバ変数 ----- //
    protected float _generic = 0.0f;
    protected List<Attribute> _buff = new();
    protected List<Attribute> _debuff = new();
    protected List<SpecialState> _special = new();
    protected float _currentState = 0.0f;

    public State(float generic)
    {
        _currentState = _generic = generic;
    }

    /// <summary> バフ・デバフの追加 </summary>
    /// <param name="attribute"> 種類 </param>
    public void AddState(Attribute attribute)
    {
        // 追加
        if (attribute.Value > 0.0f) _buff.Add(attribute);
        else _debuff.Add(attribute);

        // 更新
        GetState();
    }

    void GetState()
    {
        float buff = 0.0f;
        float debuff = 0.0f;
        float special = 0.0f;

        // バフの強いものを取得
        for (int i = _buff.Count - 1; i >= 0; i--)
        {
            // より大きい効果量のものを取得
            if (_buff[i].Value > buff)
                buff = _buff[i].Value;
        }

        // デバフの強いものを取得
        for (int i = _debuff.Count - 1; i >= 0; i--)
        {
            // より大きい効果量のものを取得
            if (_debuff[i].Value < debuff)
                debuff = _debuff[i].Value;
        }

        // 別枠の増減値を適用
        for (int i = 0; i < _special.Count; i++)
            special += _special[i].Value;

        // 0 <= xの値を代入
        _currentState = Mathf.Max(_generic + buff + debuff + special, 0.0f);
    }

    /// <summary> 特殊な数値の適用 </summary>
    /// <param name="special"> new SpecialState(x,y)等 </param>
    public void AddSpecialContent(SpecialState special)
    {
        _special.Add(special);

        // 更新
        GetState();
    }

    /// <summary> 引数と同じIDを持つ要素を削除 </summary>
    /// <param name="id"> 削除するID </param>
    public void RemoveSpecialContent(int id)
    {
        _special.RemoveAll(x => x.Id == id);

        // 更新
        GetState();
    }

    /// <summary> バフの時間の更新 </summary>
    /// <param name="remove"></param>
    public void UpdateAttribute(float remove)
    {
        // バフの更新
        for (int i = _buff.Count - 1; i >= 0; i--)
            _buff[i].RemoveTime(remove);

        // デバフの更新
        for (int i = _debuff.Count - 1; i >= 0; i--)
            _debuff[i].RemoveTime(remove);

        // バフの効果時間を減少させ、0になれば削除し更新
        int tmp = _buff.RemoveAll(x => x.Time == 0.0f);
        tmp += _debuff.RemoveAll(x => x.Time == 0.0f);

        // 削除されてるなら、更新
        if (tmp > 0) GetState();
    }
}

public class Attribute
{
    // ----- プロパティ ----- //
    public float Value => _value;
    public float Time => _time;

    // ----- メンバ変数 ----- //
    float _value = 0.0f;
    float _time = 0.0f;

    public Attribute(float value, float time)
    {
        _value = value;
        _time = time;
    }

    /// <summary> 効果時間を減少させる </summary>
    /// <param name="time"> 減少時間 </param>
    /// <returns> 効果時間が0.0fになったかどうか </returns>
    public void RemoveTime(float time)
    {
        _time = Mathf.Max(0, _time - time);
    }
}

public class SpecialState
{
    // ----- プロパティ ----- //
    public int Id => _id;
    public float Value => _value;

    // ----- メンバ変数 ----- //
    int _id;
    float _value;

    public SpecialState(int id, float value)
    {
        _id = id;
        _value = value;
    }
}

public class MoveSpeed : State
{
    public MoveSpeed(float generic) : base(generic)
    {
        _generic = generic;
    }
}

public enum StateName
{
    Speed = 0,
}