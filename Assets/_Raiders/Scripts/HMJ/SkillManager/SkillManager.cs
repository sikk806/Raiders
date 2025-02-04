using UnityEngine;

public class SkillManager : MonoBehaviour
{
  public static SkillManager instance;

    public float CoolExcel = 1;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
