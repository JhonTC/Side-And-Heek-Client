using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VisualEffectType
{
    Default,
    Positional_Ice
}

public class VisualEffects : MonoBehaviour
{
    public static Dictionary<ushort, VisualEffect> effectsList = new Dictionary<ushort, VisualEffect>();

    public static void AddEffect(ushort id, VisualEffect effect)
    {
        if (!effectsList.ContainsKey(id))
        {
            effectsList.Add(id, effect);
            PaintManager.instance.UpdatePaint();
        }
    }

    public static void RemoveEffect(ushort id)
    {
        if (effectsList.ContainsKey(id))
        {
            effectsList.Remove(id);
            PaintManager.instance.UpdatePaint();
        }
    }

    public static VisualEffect[] GetAllEffectsOfType(VisualEffectType effectType)
    {
        List<VisualEffect> retList = new List<VisualEffect>();
        foreach (var effect in effectsList.Values)
        {
            if (effect.effectType == effectType)
            {
                retList.Add(effect);
            }
        }

        return retList.ToArray();
    }

    public static PositionalVisualEffect[] GetAllPositionalEffectsOfType(VisualEffectType effectType)
    {
        List<PositionalVisualEffect> retList = new List<PositionalVisualEffect>();
        foreach (PositionalVisualEffect effect in effectsList.Values)
        {
            if (effect.effectType == effectType)
            {
                retList.Add(effect);
            }
        }

        return retList.ToArray();
    }

    public static VisualEffect CreateVisualEffectFromMessage(Message message)
    {
        VisualEffectType effectType = (VisualEffectType)message.GetInt();
        switch (effectType)
        {
            case VisualEffectType.Positional_Ice:
                return new PositionalVisualEffect(message, effectType);
            case VisualEffectType.Default:
            default:
                return new VisualEffect(effectType);
        }
    }
}

public class VisualEffect
{
    public VisualEffectType effectType;

    public VisualEffect(VisualEffectType _effectType = VisualEffectType.Default)
    {
        effectType = _effectType;
    }
}

public class PositionalVisualEffect : VisualEffect
{
    public Vector3 position;

    public PositionalVisualEffect(Vector3 _position, VisualEffectType effectType) : base(effectType)
    {
        position = _position;
    }

    public PositionalVisualEffect(Message message, VisualEffectType effectType) : base(effectType)
    {
        position = message.GetVector3();
    }
}
