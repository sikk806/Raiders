using UnityEngine;

public class Marble : MonoBehaviour
{
    public StatusEffect statusEffect;

    public void PrintMarbleData()
    {
        Debug.Log("Name : " + statusEffect.StatusEffectName);
    }
}
