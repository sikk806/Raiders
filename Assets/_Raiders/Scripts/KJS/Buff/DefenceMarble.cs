using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DefenceMarble : MonoBehaviour
{
    // Percentage (10% Up : 1.1f)
    [SerializeField] float defenceAmount = 0.9f;

    void OnEnable()
    {
        StartCoroutine("Deactive");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //other.GetComponent<BuffDebuff>().Defence(defenceAmount, 5f);
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
