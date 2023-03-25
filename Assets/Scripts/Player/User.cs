
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public static User Instance { get; private set; }
    private void Awake()
    {
        if(Instance!=null)
            throw new System.Exception("Instance is not null!");
        
        Instance = this;
    }


}


public class BirdDietData
{
    List<DietData> _userScores;
}