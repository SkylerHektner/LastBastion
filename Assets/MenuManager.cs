using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject PlayCanvas;
    public Animator Door;
    public GameObject LevelCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        PlayCanvas.SetActive(false);
        Door.SetTrigger("Open");
    }

    public void ShowLevels()
    {
        LevelCanvas.SetActive(true);
    }
}
