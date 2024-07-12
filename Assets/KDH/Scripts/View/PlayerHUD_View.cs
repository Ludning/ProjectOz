using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD_View : ViewBase<PlayerHUD_ViewModel, PlayerHUD_Message>
{
    [SerializeField] GameObject Player_Image;
    [SerializeField] List<GameObject> hp_List;
    [SerializeField] Slider PetulanceGuage_Slider;
    [SerializeField] int value = 100;

    private void Awake()
    {
        PetulanceGuage_Slider.maxValue = value;
    }

    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
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
    public void SetHp(float currentHp)
    {
        for(int i = 0;i < hp_List.Count; i++)
        {
            if (i < currentHp)
                hp_List[i].SetActive(true);
            else
                hp_List[i].SetActive(false);
        }
    }

    public void SetSwordmanMode(bool mode)
    {
        if(mode)
        {
            // 해당 경로에 집어넣을 것.
            Player_Image.GetComponent<Image>().sprite = null;
        }
        else
        {
            Player_Image.GetComponent<Image>().sprite = null;
        }
    }

    public void SetPetulanceGuage(float guage)
    {
        PetulanceGuage_Slider.value = guage;
    }


}
