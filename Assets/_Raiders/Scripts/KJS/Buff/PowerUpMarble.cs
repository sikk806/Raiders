using System.Collections;
using UnityEngine;

public class PowerUpMarble : MonoBehaviour
{
    [SerializeField] float amountPower; // default : 10% -> 0.1    

    void OnEnable()
    {
        StartCoroutine("Deactive");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<BuffDebuff>().PowerUp(amountPower, 5f);
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