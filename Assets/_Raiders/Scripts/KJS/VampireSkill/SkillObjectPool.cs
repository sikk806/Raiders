using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillObjectPools : MonoBehaviour
{
    public GameObject[] SkillSets;

    Dictionary<GameObject, bool> objs; // GameObject, bool [오브젝트 준비중인지 확인]

    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        objs = new Dictionary<GameObject, bool>();

        for(int i = 0; i < SkillSets.Count(); i++)
        {
            GameObject ob = Instantiate(SkillSets[i], transform);
            ob.name = ob.name.Replace("(Clone)", "");
            ob.SetActive(false);
            objs.Add(ob, false);
        }
    }

    public GameObject GetObject(string patternName)
    {
        // key : obj.Key
        foreach(var key in objs.Keys.ToList())
        {
            //if(key.name == patternName && !key.activeSelf && !objs[key])
            if(key.name == patternName && !key.activeSelf)
            {
                key.SetActive(true);
                //objs[key] = true;
                return key;
            }
        }

        Debug.Log($"{patternName} is not exist. (return null)");

        return null;
    }

    public void SetFalseAllObject()
    {
        foreach(GameObject ob in objs.Keys)
        {
            if(ob.activeSelf)
            {
                ob.SetActive(false);
            }
        }
    }
}
