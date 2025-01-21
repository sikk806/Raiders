using System.Collections;
using UnityEngine;

public class ManaRecoveringMarble : MonoBehaviour
{
    [SerializeField] float mamaRecoveringAmount = 2;

    void OnEnable()
    {
        StartCoroutine("Deactive");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //other.GetComponent<BuffDebuff>().ManaRecovering(mamaRecoveringAmount, 5f);
            other.GetComponent<BuffDebuff>().EffectOn(gameObject);
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }

    IEnumerator Deactive()
    {
        yield return new WaitForSeconds(4.9f);

        gameObject.SetActive(false);
    }
}
