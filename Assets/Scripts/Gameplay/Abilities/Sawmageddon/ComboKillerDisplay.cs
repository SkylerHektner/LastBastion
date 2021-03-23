using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboKillerDisplay : MonoBehaviour
{
    public static ComboKillerDisplay Instance;
    [SerializeField] private HpBar HPBar;
    private void Start()
    {
        Instance = this;
        HPBar.SetSize( 0.0f );
        gameObject.SetActive( false );
    }

    public void SetChargeAmount(int current, int max)
    {
        HPBar.SetSize( (float)current / (float)max );
    }
}
