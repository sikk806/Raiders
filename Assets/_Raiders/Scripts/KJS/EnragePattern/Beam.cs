using System.Collections;
using UnityEngine;

public class Beam : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine("Deactive");
    }

    void OnDisable()
    {
        gameObject.SetActive(false);
    }

    IEnumerator Deactive()
    {
        yield return new WaitForSeconds(1.75f);
        gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Hp>().TakeDamage(0.1f);
        }
    }
}
