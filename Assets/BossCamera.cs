﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCamera : MonoBehaviour
{
    public GameObject VoidLevel;
    public GameObject OverworldLevel;
    public GameObject Hourglass;


    public void LoadVoid()
    {
        OverworldLevel.SetActive(false);
        VoidLevel.SetActive(true);
    }

    public void LoadOverworld()
    {
        OverworldLevel.SetActive(true);
        VoidLevel.SetActive(false);
    }

    public void ShowHourGlass()
    {
        Hourglass.SetActive(true);
    }
    public void HideHourGlass()
    {
        Hourglass.SetActive(false);
    }
}
