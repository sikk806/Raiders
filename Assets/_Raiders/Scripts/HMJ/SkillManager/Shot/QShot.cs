using System.Threading;
using UnityEngine;
using UnityEngine.Purchasing;

public class QShot : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {

            Enemy enemy = other.GetComponent<Enemy>();
            enemy.TakeDamage(Q.Instance.CalculatingDamage);
        }

    }

}
