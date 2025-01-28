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
        Debug.Log("Please check scene name");
        SceneManager.LoadScene("");
    }

    public void OnExitPressed()
    {
        Popup.SetActive(false);
        Player.Instance.BringBackControl();
    }
}
