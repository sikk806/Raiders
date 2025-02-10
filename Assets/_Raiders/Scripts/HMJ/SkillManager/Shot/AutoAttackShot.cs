using UnityEngine;

public class AutoAttackShot : MonoBehaviour
{
    private BoxCollider collider;
    Vector3 vector3 = new Vector3(0, 2.5f, 1);
    private void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        
        if (other.CompareTag("Enemy") || other.CompareTag("Boss1") || other.CompareTag("Boss2"))
        {
            Player.Instance.DrainHp();
            Hp enemy = other.GetComponent<Hp>();
            enemy.TakeDamage(AutoAttack.CalculatingDamage());
            Damage.Instance.ShowDamage((int)AutoAttack.CalculatingDamage(), other.transform.position + vector3);

        }
        else if (other.CompareTag("Barrier"))
        {
            var a =other.GetComponent<PatternSc>().hp;
            a.TakeDamage(AutoAttack.CalculatingDamage());

            Debug.Log("베리어맞음");
        }

    }
}
