using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EquipInformation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    CustomizationItem[] MyCosmetics;
    public GameObject ContentList;
    public TextMeshProUGUI ItemName;
    public Image ShowcaseImage;

    void OnEnable()
    {
        MyCosmetics = ContentList.GetComponentsInChildren<CustomizationItem>();
        foreach (CustomizationItem Cosmetic in MyCosmetics)
        {
            if (Cosmetic.ActivatedFX.activeInHierarchy)
            {
                ItemName.text = Cosmetic.MyCosmetic.Name;
                ShowcaseImage.sprite = Cosmetic.MyIcon.sprite;

                // also display large image of it here TODO
            }
        }
    }

}
