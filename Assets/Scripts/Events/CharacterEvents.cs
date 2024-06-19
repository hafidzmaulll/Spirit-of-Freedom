using UnityEngine;
using UnityEngine.Events;

public static class CharacterEvents
{
    // Character damaged and damage value
    public static UnityAction<GameObject, float> characterDamaged = delegate { };

    // Character healed and amount healed
    public static UnityAction<GameObject, float> characterHealed = delegate { };
}
