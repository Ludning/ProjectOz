using Sirenix.Utilities;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD_View : ViewBase<PlayerHUD_ViewModel, PlayerHUD_Message>
{
    [SerializeField] Image Player_Image;
    [SerializeField] Slider PetulanceGuage_Slider;
    [SerializeField] int value = 100;

    [SerializeField] List<GameObject> hp_List;
    [SerializeField] GenerateHP generateHP;

    private void Start()
    {
        PetulanceGuage_Slider.maxValue = value;
        hp_List = new List<GameObject>();
        hp_List = generateHP.hp_List;
    }

    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PlayerHUD_ViewModel.SwordmanMode):
                SetSwordmanMode(_vm.SwordmanMode);
                break;
            case nameof(PlayerHUD_ViewModel.PlayerHp):
                SetHp(_vm.PlayerHp);
                break;
            case nameof(PlayerHUD_ViewModel.PetulanceGuage):
                SetPetulanceGuage(_vm.PetulanceGuage);
                break;
        }
    }
    private void SetHp(float currentHp)
    {
        for (int i = 0; i < hp_List.Count; i++)
        {
            
            if (i < currentHp)
            {
                hp_List[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                hp_List[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    private void SetSwordmanMode(bool mode)
    {
        string modeName;

        if (mode)
            { modeName = "Knight_Mode_UI"; }
        else
            { modeName = "Mage_Mode_UI"; }

        Player_Image.sprite = ResourceManager.Instance.LoadResource<Sprite>(modeName);
    }

    private void SetPetulanceGuage(float guage)
    {
        PetulanceGuage_Slider.value = guage;
    }


}
