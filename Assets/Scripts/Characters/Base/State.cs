using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class State
{
    // ----- プロパティ ----- //
    public float Generic => _generic;
    public List<Attribute> Buff => _buff;
    public List<Attribute> Debuff => _debuff;
    public float CurrentState => ReturnValue(_currentState);

    // ----- メンバ変数 ----- //
    protected float _generic = 0.0f;
    protected List<Attribute> _buff = new();
    protected List<Attribute> _debuff = new();
    protected float _currentState = 0.0f;
    protected bool mode = true;
    protected List<float> special = new();

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

    protected float GetState()
    {
        float buff = 0.0f;
        float debuff = 0.0f;

        // バフの強いものを取得
        for (int i = _buff.Count - 1; i >= 0; i--)
        {
            if (_buff[i].Value > buff)
            {
                // 効果量を取得、効果時間が切れたなら削除
                buff = _buff[i].Value;
                if (_buff[i].Time < 0.0f) _buff.RemoveAt(i);
            }
        }

        // デバフの強いものを取得
        for (int i = _debuff.Count - 1; i >= 0; i--)
        {
            if (_debuff[i].Value < debuff)
            {
                // 効果量を取得、効果時間が切れたなら削除
                debuff = _debuff[i].Value;
                if (_debuff[i].Time < 0.0f) _debuff.RemoveAt(i);
            }
        }

        float sum_sp = 0.0f;
        foreach(float sp in special) sum_sp += sp;
        return ReturnValue(_currentState = Mathf.Max(_generic + buff + debuff + sp, 0.0f));
    }

    protected float ReturnValue(float value)
    {
        // 0 <= xの値を返す
        if (mode) return value;
        else return 0.0f;
    }

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

    public void Mode(bool move)
    {
        mode = move;
        GetState();
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