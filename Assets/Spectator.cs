using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{

    public static int LevelIndex;
    public static bool ReturningFromLevel;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Spectator");

        if (objs.Length > 1) // no duplicate spectators
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

}
