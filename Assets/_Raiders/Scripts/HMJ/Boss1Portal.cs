using TMPro;
using UnityEngine;

public class Boss1Portal : MonoBehaviour
{
    [SerializeField]
    GameObject Popup; //The Popup which active when 'player' triggered

    [SerializeField]
    TextMeshProUGUI PopupText; //The Popup's text

    private void OnParticleCollision(GameObject other)
    {
        Popup.SetActive(true);
    }
}
