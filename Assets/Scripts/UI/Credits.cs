using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    Animator CreditsAnimator;

    // Start is called before the first frame update
    void Start()
    {
        CreditsAnimator = gameObject.GetComponent<Animator>();
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }

    public void TriggerOutroTransition()
    {
        CreditsAnimator.SetTrigger("Exit");
        Invoke("LoadMenuScene", 5f);
    }


}
