using TMPro;
using UnityEngine;

public class ATK : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI atkText;
    int defaultAddedPower;


    void Start()
    {
        atkText = GetComponentInChildren<TextMeshProUGUI>();
        atkText.text = $"ATK: {Player.Instance.Power} (+{(int)Player.Instance.AddedPower})";
        defaultAddedPower = (int)Player.Instance.AddedPower;
    }
}
