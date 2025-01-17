using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    public static AutoAttack Instance;
    public bool IsUsable = true;
    public float CoolTime = 0.3f;
    public float SkillDamage = 90f;
    public float CalculatingDamage;
    public float UseMp = 0;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        CalculatingDamage = SkillDamage + Player.Instance.Power + Player.Instance.AddedPower;
    }
}
