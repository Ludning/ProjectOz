using UnityEngine;
using UnityEngine.UI;

public class SyncPlaceHolder : MonoBehaviour
{
    [SerializeField] private Text placeHolder;
    [SerializeField] private Text header;
    //private void OnValidate()
    //{
    //    transform.parent.GetChild(0)
    //        .TryGetComponent(out placeHolder);
    //    transform.parent.GetChild(1)
    //        .TryGetComponent(out header);
    //}
    private void OnEnable()
    {
        header.text = placeHolder.text;
    }

}
