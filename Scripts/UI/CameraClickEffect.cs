using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraClickEffect : PopupUI
{
    [SerializeField] private Image image;

    private void OnEnable()
    {
        StartCoroutine(PlayEffect());
    }

    IEnumerator PlayEffect()
    {
        image.DOFade(1f, 0.1f);
        yield return YieldInstructionCache.WaitForSeconds(0.1f); //new WaitForSeconds(0.1f);
        image.DOFade(0f, 1f);
    }

    public override void OnClickExit()
    {

    }
}
