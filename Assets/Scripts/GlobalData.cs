using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THERE SHOULD ONLY BE ONE GLOBAL DATA STRUCTURE
[CreateAssetMenu( fileName = "Achievement", menuName = "ScriptableObjects/GlobalData", order = 0 )]
public class GlobalData : ScriptableObject
{
    public List<Achievement> achievements;
}
