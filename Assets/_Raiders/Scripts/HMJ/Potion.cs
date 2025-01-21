using TMPro;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public static Potion Instance;
    public float HpHeal = 50f; // The amount of Hp recovery
    public float MpHeal = 500f; //The amount of Mp recovery
    public int HpPotion; //The number of Hp potion left
    public int MpPotion; //The number of Mp potion left
    public TextMeshProUGUI HpPotionText; // HpPotion's text UI
    public TextMeshProUGUI MpPotionText; // MpPotion's text UI

    [SerializeField]
    GameObject NoPotionWarning;

    void Start()
    {
        Instance = this;

        HpPotion = 5; 
        MpPotion = 5;

        HpPotionText.text = HpPotion.ToString();
        MpPotionText.text = MpPotion.ToString();
    }

    public void NoPotion()
    {
        NoPotionWarning.SetActive(true);
        Invoke("WarningClose",1.5f);
    }

    void WarningClose()
    {
        NoPotionWarning.SetActive(false);
    }

}
