using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{

    public string SceneToLoad;
    public bool locked;
    public GameObject LockedSymbol;


    public void Update()
    {
        if (locked)
        {
            LockedSymbol.SetActive(true);
            gameObject.GetComponent<Button>().enabled = false;
        }
        else
        {
            LockedSymbol.SetActive(false);
            gameObject.GetComponent<Button>().enabled = true;
        }
    }

    public void LoadMyLevel()
    {
        SceneManager.LoadScene(SceneToLoad);
    }
}
