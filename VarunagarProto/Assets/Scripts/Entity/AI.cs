using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    public static AI SINGLETON { get; private set; }
    

    void Awake()
    {
        if (SINGLETON != null)
        {
            Debug.LogError("Trying to instantiate another CombatManager SINGLETON");
            Destroy(gameObject);
            return;
        }
        SINGLETON = this;
        DontDestroyOnLoad(gameObject);
    }
    
    
    
}
