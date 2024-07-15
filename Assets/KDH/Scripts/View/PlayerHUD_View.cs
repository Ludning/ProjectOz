using Sirenix.Utilities;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD_View : ViewBase<PlayerHUD_ViewModel, PlayerHUD_Message>
{
    [SerializeField] Image Player_Image;
    [SerializeField] List<GameObject> hp_List;
    [SerializeField] Slider PetulanceGuage_Slider;
    [SerializeField] int value = 100;

    private void Awake()
    {
        PetulanceGuage_Slider.maxValue = value;
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
                hp_List[i].SetActive(true);
            else
                hp_List[i].SetActive(false);
        }
    }

    private void SetSwordmanMode(bool mode)
    {
        string modeName;

        if (mode)
        { modeName = "sword"; }
        else
        { modeName = "broom"; }

        Player_Image.sprite = ResourceManager.Instance.LoadResource<Sprite>(modeName);
    }

    private void SetPetulanceGuage(float guage)
    {
        PetulanceGuage_Slider.value = guage;
    }


}
