using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizationItem : MonoBehaviour
{

    public GameObject MyChains;
    public string TypeID;
    public GameObject MyButton;
    public Image ColoredBG;
    public TextMeshProUGUI DescriptorText;
    public Cosmetic CosmeticInfo; // get rid of this later
    public bool Purchased // toggles chains on/off if purchased or not
    {
        get
        {
            return ItemPurchased;
        }
        set
        {
            ItemPurchased = value;

            if (ItemPurchased)
            {
                MyChains.SetActive(false);
                MyButton.SetActive(true);
                ColoredBG.color = Color.black;
            }
            else // not bought yet
            {
                MyChains.SetActive(true);
                //MyButton.SetActive(false);
                ColoredBG.color = Color.grey;
            }
        }
    }
    private bool ItemPurchased;
    public bool CurrentlyEquipped
    {
        get
        {
            return Equipped;
        }
        set
        {
            Equipped = value;

            if (Equipped)
            {
                ColoredBG.color = Color.green;
            }
            else
            {
                ColoredBG.color = Color.black;
            }
        }
    }
    private bool Equipped;




    public void RattleChains() // toggles purchased status of item
    {
        if (Purchased == false)
        {
            MyChains.GetComponent<Animator>().SetTrigger("Rattle");
        }
        Debug.Log("Purchased: " + Purchased);
    }
    [ContextMenu("ToggleActive")]
    public void ToggleActive() // toggles current active item
    {
        if (Purchased)
        {
            CurrentlyEquipped = !CurrentlyEquipped;
        }
        Debug.Log("Currently Active?: " + CurrentlyEquipped);
    }

    [ContextMenu("DebugPurchaseMe")]
    public void DebugPurchaseMe()
    {
        Purchased = true;
    }

    public void UpdateDescriptorText()
    {
        DescriptorText.text = CosmeticInfo.Description;
    }

    public void WipeDescriptorText()
    {
        DescriptorText.text = "";
    }

}
