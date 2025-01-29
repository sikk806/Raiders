using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeftPortal : MonoBehaviour
{
    [SerializeField]
    GameObject Popup; //The Popup which active when 'player' triggered

    private void OnTriggerEnter(Collider other)
    {
        Popup.SetActive(true);
        Player.Instance.TakeControl();
    }

    public void OnEnterPressed()
    {
        SceneManager.LoadScene("Boss1Scene");
    }

    public void OnExitPressed()
    {
        Popup.SetActive(false);
        Player.Instance.BringBackControl();
    }
}
