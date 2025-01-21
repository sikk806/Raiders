using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    public static bool IsUsable = true;
    public float CoolTime = 0.3f;
    public static float SkillDamage = 90f;
    public float UseMp = 0;

 
    public static float CalculatingDamage()
    {
        return SkillDamage + Player.Instance.Power + Player.Instance.AddedPower;
    }
}

