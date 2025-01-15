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
        //if (other.CompareTag("Enemy"))
        //{

        //    Enemy enemy = other.GetComponent<Enemy>();
        //    if (enemy != null)
        //    {

        //        enemy.Instance.CurrentHp -= AutoAttack.Instance.CalculatingDamage;
        //    }
        //    else
        //    {
        //        Debug.LogWarning("There is No 'Enemy' Script in target");
        //    }
        //}

    }
}
