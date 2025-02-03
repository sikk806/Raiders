using UnityEngine;

public class ImpactCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<Hp>().TakeDamage(500000f);
            // GameManager.Instance.DeathCount = 1;
        }
    }
}
