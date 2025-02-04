using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeftPortal : MonoBehaviour
{
    [SerializeField]
    GameObject Popup; //The Popup which active when 'player' triggered

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Popup.SetActive(true);
            Player.Instance.TakeControl();
        }
    }

    public void OnEnterPressed()
    {
        Player.Instance.BringBackControl();
        SceneManager.LoadScene("Boss1Scene");
    }

    public void OnExitPressed()
    {
        Popup.SetActive(false);
        Player.Instance.BringBackControl();
    }
}
