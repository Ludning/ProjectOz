using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateHP : MonoBehaviour
{
    [SerializeField] int size = 5; //HP юс╫ц
    [SerializeField] int Icon_Size = 50;
    [SerializeField] int padding_length = 50;

    public List<GameObject> hp_List;

    private void Awake()
    {
        hp_List = new List<GameObject> ();
        CreateHP();
    }

    void CreateHP()
    {
        GameObject nullObj = new GameObject();

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(nullObj, gameObject.transform.position + Vector3.right * padding_length * i, gameObject.transform.rotation, transform);
            
            obj.name = "HP";

            var obj_HPEmptyImage = Instantiate(nullObj, obj.transform);
            obj_HPEmptyImage.name = "HP_Empty";
            Image emptyImg = obj_HPEmptyImage.AddComponent<Image>();
            emptyImg.rectTransform.localScale = Vector3.one * 0.5f;
            emptyImg.sprite = ResourceManager.Instance.LoadResource<Sprite>("Hp_UI_Del");

            var obj_HPFullImage = Instantiate(nullObj, obj.transform);
            obj_HPFullImage.name = "HP_Full";
            Image fullImg = obj_HPFullImage.AddComponent<Image>();
            fullImg.rectTransform.localScale = Vector3.one * 0.5f;
            fullImg.sprite = ResourceManager.Instance.LoadResource<Sprite>("Hp_UI");

            hp_List.Add(obj);
        }
    }
}
