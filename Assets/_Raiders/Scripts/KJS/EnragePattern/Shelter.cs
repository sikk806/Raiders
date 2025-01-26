using TMPro;
using UnityEngine;

public class Shelter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            EnragePattern ep = GetComponentInParent<EnragePattern>();
            if (ep != null)
            {
                Debug.Log("Safe..!!!");
                ep.IsSafe = true;
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
            EnragePattern ep = GetComponentInParent<EnragePattern>();
            if (ep != null)
            {
                ep.IsSafe = false;
            }
            else
            {
                Debug.Log("No Ep Component - Shelter.cs");
            }
        }
    }
}
