using TMPro;
using UnityEngine;

public class Shelter : MonoBehaviour
{
    float OriginDamage = 0;
    EnragePattern EP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EP = GetComponentInParent<EnragePattern>();
        OriginDamage = EP.Damage;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (EP != null)
            {
                Debug.Log("Safe..!!!");
                EP.Damage = 0f;
            }
            else
            {
                Debug.Log("No Ep Component - Shelter.cs");
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (EP != null)
            {
                EP.Damage = OriginDamage;
            }
            else
            {
                Debug.Log("No Ep Component - Shelter.cs");
            }
        }
    }
}
