using System.Threading;
using UnityEngine;
using UnityEngine.Purchasing;

public class AutoAttackShot : MonoBehaviour
{
    private BoxCollider collider;
    private void Start()
    {
       collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

            Enemy enemy = other.GetComponent<Enemy>();
            enemy.TakeDamage(AutoAttack.Instance.CalculatingDamage);
        }

    }
}
