using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoItem : MonoBehaviour
{
    public UnlockFlagUIInformation UnlockFlagInformation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetInfoName()
    {
        return UnlockFlagInformation.UnlockName;
    }
    public string GetInfoDescription()
    {
        return UnlockFlagInformation.Description;
    }
}
