using UnityEngine;

public class ImpactCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Damaged.. YouDie.");
            // GameManager.Instance.DeathCount = 1;
            // other.GetComponent<Hp>().TakeDamage(500000f);
        }
    }
}
