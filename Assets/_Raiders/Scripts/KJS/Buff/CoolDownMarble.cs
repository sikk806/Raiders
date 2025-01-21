using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CoolDownMarble : MonoBehaviour
{
    [SerializeField] float coolDownAmount = 2;

    void OnEnable()
    {
        StartCoroutine("Deactive");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //other.GetComponent<BuffDebuff>().CoolDown(coolDownAmount, 5f);
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
