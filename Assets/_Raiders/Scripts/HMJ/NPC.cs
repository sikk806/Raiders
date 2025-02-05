using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    bool isTriggered;

    [SerializeField]
    GameObject messageBox;

    [SerializeField]
    TextMeshProUGUI messageText;

    [SerializeField]
    GameObject cardSelect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = false;
        }

    }

    private void Update()
    {
        if (isTriggered && !messageBox.activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            if (CardSelect.instance.cardSelectCnt != 0)
            {
                StartCoroutine(ShowMessageSequence());
            }
        }

        if (CardSelect.instance.cardSelectCnt == 0 && messageBox.activeSelf)
        {
            Player.Instance.BringBackControl();
            messageBox.SetActive(false);

        }
    }

    private IEnumerator ShowMessageSequence()
    {
        messageText.text = "신성한 물의 기운이 느껴지는 곳이다..\n전투에 들어가기 전, 도움을 받을 수 있을 것 같다.";
        messageBox.SetActive(true);
        Player.Instance.CurrentState = PlayerState.Idle;
        Player.Instance.TakeControl();
        

        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.F));

        messageText.text = "카드를 선택해주세요.";
        cardSelect.SetActive(true);
    }
}
