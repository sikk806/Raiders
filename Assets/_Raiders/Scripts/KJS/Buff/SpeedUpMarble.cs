using System.Collections;
using UnityEngine;

public class SpeedUpMarble : MonoBehaviour
{
    // Percentage (10% Up : 1.1f)
    [SerializeField] float SpeedUpAmount = 1.25f;
    
    void OnEnable()
    {
        StartCoroutine("Deactive");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //other.GetComponent<BuffDebuff>().SpeedUp(SpeedUpAmount, 5f);
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