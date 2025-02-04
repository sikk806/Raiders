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
        Debug.Log(other.gameObject.name);
        
        if (other.CompareTag("Enemy") || other.CompareTag("Boss1") || other.CompareTag("Boss2"))
        {
            
            Debug.Log(other.name);
            Player.Instance.DrainHp();
            Hp enemy = other.GetComponent<Hp>();
            enemy.TakeDamage(AutoAttack.CalculatingDamage());
        }
        else if (other.CompareTag("Barrier"))
        {
            var a =other.GetComponent<PatternSc>().hp;
            a.TakeDamage(AutoAttack.CalculatingDamage());
            
            Debug.Log("베리어맞음");
        }

    }
}
