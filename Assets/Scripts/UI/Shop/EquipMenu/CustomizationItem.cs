using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CustomizationItem : MonoBehaviour
{
    public static UnityEvent EquipChanged = new UnityEvent();

    public GameObject MyChains;
    public GameObject MyButton;
    public Image ColoredBG;
    public TextMeshProUGUI DescriptorText;
    public Cosmetic MyCosmetic;

    public void Start()
    {
        PD.Instance.UnlockFlagChangedEvent.AddListener( OnUnlockFlagChanged );
        EquipChanged.AddListener( OnEquipChanged );
    }

    public void OnDestroy()
    {
        PD.Instance.UnlockFlagChangedEvent.RemoveListener( OnUnlockFlagChanged );
        EquipChanged.RemoveListener( OnEquipChanged );
    }

    public void Awake()
    {
        UpdatePurchasedGraphics();
        UpdateEquippedGraphics();
    }

    public void RattleChains()
    {
        if( !MyCosmetic.IsUnlocked() )
        {
            MyChains.GetComponent<Animator>().SetTrigger( "Rattle" );
        }
    }

    public void ToggleActive()
    {
        if( MyCosmetic.IsUnlocked() && !MyCosmetic.IsEquipped() )
        {
            MyCosmetic.Equip();
            EquipChanged.Invoke();
        }
    }

    public void UpdateDescriptorText()
    {
        DescriptorText.text = MyCosmetic.Name;
    }

    public void WipeDescriptorText()
    {
        DescriptorText.text = "";
    }

    private void UpdatePurchasedGraphics()
    {
        if( MyCosmetic.IsUnlocked() )
        {
            MyChains.SetActive( false );
            MyButton.SetActive( true );
            ColoredBG.color = Color.black;
        }
        else
        {
            MyChains.SetActive( true );
            ColoredBG.color = Color.grey;
        }
    }

    private void UpdateEquippedGraphics()
    {
        if( MyCosmetic.IsEquipped() )
        {
            ColoredBG.color = Color.green;
        }
        else
        {
            ColoredBG.color = Color.black;
        }
    }

    private void OnUnlockFlagChanged( UnlockFlag flag, bool new_value )
    {
        if( MyCosmetic.unlock_flag == flag )
        {
            if( !new_value && MyCosmetic.IsEquipped() )
            {
                MyCosmetic.UnEquip();
                UpdateEquippedGraphics();
            }
            else
            {
                UpdatePurchasedGraphics();
            }
        }
    }

    private void OnEquipChanged()
    {
        UpdateEquippedGraphics();
    }
}
