using System;
using UnityEngine;

public class Identicator : MonoBehaviour
{
    public int  identicator;
    
    
    
    private void OnDisable()
    {
        var pattern = FindObjectOfType<Pattern>();
        if (pattern != null)
        {
            pattern.DestroyStatue(gameObject);
        }
    }
    
  
    
}
