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
    float ChainsSFXCooldown;
    bool ChainsCanRattle;
    public GameObject ActivatedFX;
    public Image MyIcon;
    public Image ShowcaseImage;
    public EquipInformation ParentPortal;

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
    public void FixedUpdate()
    {
        if (ChainsCanRattle == false) // delays the chains from being spammed
        {
            ChainsSFXCooldown -= Time.smoothDeltaTime;
            if (ChainsSFXCooldown <= 0)
            {
                ChainsSFXCooldown = 0; // no negatives pls
                ChainsCanRattle = true;
            }
        }
    }

    public void RattleChains()
    {
        if( !MyCosmetic.IsUnlocked() && ChainsCanRattle)
        {
            MyChains.GetComponent<Animator>().SetTrigger( "Rattle" );
            ChainsCanRattle = false;
            ChainsSFXCooldown = 1f;
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
        ShowcaseImage.sprite = MyIcon.sprite;
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
            ActivatedFX.SetActive(true);
            gameObject.GetComponent<AudioSource>().Play();
            ParentPortal.SetPreview(gameObject.GetComponent<CustomizationItem>());
        }
        else
        {
            ColoredBG.color = Color.black;
            ActivatedFX.SetActive(false);
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
