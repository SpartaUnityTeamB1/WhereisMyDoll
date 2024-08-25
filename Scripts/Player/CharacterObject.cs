using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObject : MonoBehaviour
{
    private Vector3 currentScale;
    private Tween tween;

    public void SpawnCharacter()
    {
        currentScale = transform.localScale;

        tween = transform.DOScale(currentScale * 1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void DespawnCharacter()
    {
        tween.Kill();

        transform.localScale = Vector3.one;
    }
}
