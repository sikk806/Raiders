using TMPro;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public static Potion Instance;
    public float HpHeal = 500f; // The amount of Hp recovery
    public float MpHeal = 500f; //The amount of Mp recovery
    public int HpPotion = 5; //The number of Hp potion left
    public int MpPotion = 5; //The number of Mp potion left
    public TextMeshProUGUI HpPotionText; // HpPotion's text UI
    public TextMeshProUGUI MpPotionText; // MpPotion's text UI

    void Start()
    {
        Instance = this;

        HpPotionText.text = HpPotion.ToString();
        MpPotionText.text = MpPotion.ToString();
    }

}
