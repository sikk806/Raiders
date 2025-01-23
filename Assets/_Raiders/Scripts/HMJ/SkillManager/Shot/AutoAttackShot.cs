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
        if (other.CompareTag("Enemy") || other.CompareTag("Boss1") || other.CompareTag("Boss2"))
        {
            Player.Instance.DrainHp();
            Hp enemy = other.GetComponent<Hp>();
            enemy.TakeDamage(AutoAttack.CalculatingDamage());
        }

    }
}
