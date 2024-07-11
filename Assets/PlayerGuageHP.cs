using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGuageHP : MonoBehaviour
{
    List<GameObject> hp_List;

    public void SetHp(int currentHp)
    {
        for(int i = 0;i < hp_List.Count; i++)
        {
            if (i < currentHp)
                hp_List[i].SetActive(true);
            else
                hp_List[i].SetActive(false);
        }
    }
}
