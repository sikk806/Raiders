using System.Collections;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    // GM에서 처리할 수 있도록 변경할 것.
    [SerializeField] SkillObjectPools SkillObjectPool;

    IEnumerator GroundScratching()
    {
        yield return null;
        GameObject GS = SkillObjectPool.GetObject("GroundScratching");
        GS.transform.position = transform.position;
        GS.transform.position = new Vector3(GS.transform.position.x, 2f, GS.transform.position.z);
        GS.transform.rotation = new Quaternion(0f, transform.rotation.y, 0f, transform.rotation.w);
    }

    IEnumerator TargetSpell()
    {
        int cnt = 0;
        while (cnt < 5)
        {
            GameObject TS = SkillObjectPool.GetObject("TargetSpell");
            Transform targetTransform = GetComponent<BehaviourAI>().TargetPlayer.transform;
            TS.transform.position = targetTransform.position;
            cnt++;
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator Explosion()
    {
        yield return null;
        GameObject EP = SkillObjectPool.GetObject("Explosion");
        EP.transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
    }

    IEnumerator BloobBoom()
    {
        int cnt = 0;
        while (cnt < 20)
        {
            GameObject BB = SkillObjectPool.GetObject("BloodBoom");
            float x = Random.Range(-10f, 10f);
            float zMax = 10 - Mathf.Abs(x);
            float z = Random.Range(-zMax, zMax);
            BB.transform.position = new Vector3(x, 2f, z);
            cnt++;
            yield return new WaitForSeconds(0.05f);

        }
    }
    
    
    //이게 보스 주위에있는베리어
    public void Barrier()
    {
        GameObject BB = SkillObjectPool.GetObject("Barrier");
        BB.transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
    }

 
    
    

}
