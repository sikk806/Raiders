using TMPro;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public float HpHeal = 50f; // The amount of Hp recovery
    public float MpHeal = 300f; //The amount of Mp recovery
    public int HpPotion; //The number of Hp potion left
    public int MpPotion; //The number of Mp potion left

    [SerializeField]
    TextMeshProUGUI HpPotionText; // HpPotion's text UI
    [SerializeField]
    TextMeshProUGUI MpPotionText; // MpPotion's text UI

    void Start()
    {
        HpPotion = 5;
        MpPotion = 5;
    }

}
