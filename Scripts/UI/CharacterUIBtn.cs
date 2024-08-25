using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] private GameObject crossObject;
    [SerializeField] private Button chaButton;
    private bool isCross = false;

    private void Start()
    {
        chaButton.onClick.AddListener(OnChaBtnClick);
    }

    public void OnChaBtnClick()
    {
        if (!isCross)
        {
            crossObject.SetActive(true);
            isCross = true;
        }
        else
        {
            crossObject.SetActive(false);
            isCross = false;
        }
    }
}
