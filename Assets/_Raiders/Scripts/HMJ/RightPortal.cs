using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RightPortal : MonoBehaviour
{
    [SerializeField]
    GameObject Popup; //The Popup which active when 'player' triggered

    private void OnTriggerEnter(Collider other)
    {
        Popup.SetActive(true);
        Player.Instance.TakeControl();
    
    }
    public void OnButtonPressed()
    {
        Popup.SetActive(false);
        Player.Instance.BringBackControl();
    }
}
