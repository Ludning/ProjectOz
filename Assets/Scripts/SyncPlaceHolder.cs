using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SyncPlaceHolder : MonoBehaviour
{
    private Text placeHolder;
    private Text header;
    private void Awake()
    {
        transform.parent.GetChild(0)
            .TryGetComponent(out placeHolder);
        transform.parent.GetChild(1)
            .TryGetComponent(out header);
    }
    private void OnEnable()
    {
        header.text = placeHolder.text;
    }

}
