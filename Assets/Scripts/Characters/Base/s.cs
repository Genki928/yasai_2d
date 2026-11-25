using System;
using System.Collections.Generic;

public class Attribute<T> 
{
    protected T generic;
}

public class Effect<T>
{
    public T Value => value;
    public int Id => id;
    T value;
    float timer = 0.0f;
    int id = 0;
    bool isDelete = true;

    public Effect(T value, float timer, int id = 0, bool isDelete = true)
    {
        this.value = value;
        this.timer = timer;
        this.id = id;
        this.isDelete = isDelete;
    }

    /// <returns> éûä‘êÿÇÍandè¡ãéâ¬î\Ç»ÇÁTrue </returns>
    public bool EffectUpdate(float deltaTime)
    {
        timer = MathF.Max(0, timer - deltaTime);
        return timer == 0.0f && isDelete;
    }
}

public class ModifierAttribute<T> : Attribute<T>
{
    protected List<Effect<T>> effects = new();

    public void AttributeUpdate(float deltaTime)
    {
        effects.RemoveAll(x => x.EffectUpdate(deltaTime));
    }

    public void AddEffect(Effect<T> effect)
    {
        effects.Add(effect);
    }

    public void RemoveEffect(int id)
    {
        effects.RemoveAll(x => x.Id == id);
    }
}

public class MoveSpeed : ModifierAttribute<float>
{
    public float Value
    {
        get
        {
            float total = 0.0f;
            foreach (var effect in effects)
                total += effect.Value;
            return generic + total;
        }
    }

    public MoveSpeed(float baseValue)
    {
        generic = baseValue;
    }
}