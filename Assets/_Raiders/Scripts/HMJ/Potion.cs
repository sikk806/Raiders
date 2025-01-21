using TMPro;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public static Potion Instance;
    public float HpHeal = 50f; // The amount of Hp recovery
    public float MpHeal = 500f; //The amount of Mp recovery
    public int HpPotion; //The number of Hp potion left
    public int MpPotion; //The number of Mp potion left

    [SerializeField]
    TextMeshProUGUI HpPotionText; // HpPotion's text UI
    [SerializeField]
    TextMeshProUGUI MpPotionText; // MpPotion's text UI

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
        Debug.Log("포션 부족 안내 팝업 띄우기");
    }

}
