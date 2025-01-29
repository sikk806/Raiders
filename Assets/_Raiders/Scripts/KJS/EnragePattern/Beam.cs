using System.Collections;
using UnityEngine;

public class Beam : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine("Deactive");
    }

    IEnumerator Deactive()
    {
        yield return new WaitForSeconds(1.9f);
        gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Damaged");
        }
    }
}
