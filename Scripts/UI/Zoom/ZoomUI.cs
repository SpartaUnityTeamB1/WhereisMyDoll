using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomUI : BaseUI
{
    [SerializeField] private GameObject zoomImg;
    private RectTransform zoomImgRect;

    private float x;
    private float y;

    private float tmpCursorPosX;
    private float tmpCursorPosY;

    private float minWidth;
    private float maxWidth;
    private float minHeight;
    private float maxHeight;

    private float defaultSize = 7.7f;
    private float changeSize = 1.57f;

    private bool isCoroutineRunning = false;


    private void Start()
    {
        zoomImgRect = zoomImg.GetComponent<RectTransform>();
    }

    private void Update()
    {
        x = Input.mousePosition.x - (Screen.width / 2);
        y = Input.mousePosition.y - (Screen.height / 2);

        zoomImgRect.localPosition = new Vector2(x, y);

        tmpCursorPosX = zoomImgRect.localPosition.x;
        tmpCursorPosY = zoomImgRect.localPosition.y;

        minWidth = -Screen.width / 2;
        maxWidth = Screen.width / 2;
        minHeight = -Screen.height / 2;
        maxHeight = Screen.height / 2;

        if (2 == GameManager.Instance.Player.IsZoom)
        {
            tmpCursorPosX = Mathf.Clamp(tmpCursorPosX, minWidth + 500, maxWidth - 500);
            tmpCursorPosY = Mathf.Clamp(tmpCursorPosY, minHeight + 250, maxHeight - 250);
        }
        else
        {
            tmpCursorPosX = Mathf.Clamp(tmpCursorPosX, minWidth + 200, maxWidth - 200);
            tmpCursorPosY = Mathf.Clamp(tmpCursorPosY, minHeight + 200, maxHeight - 200);
        }

        zoomImgRect.localPosition = new Vector2(tmpCursorPosX, tmpCursorPosY);
    }

    public void Zoom()
    {
        zoomImg.transform.DOScale(changeSize, 0.5f).SetEase(Ease.OutQuad);
    }

    public void ReleaseZoom()
    {
        if (!isCoroutineRunning && gameObject.activeInHierarchy)
            StartCoroutine(ReleaseZoomCor());
    }

    private IEnumerator ReleaseZoomCor()
    {
        isCoroutineRunning = true;

        GameManager.Instance.Player.IsZoom = 1;

        var tween = zoomImg.transform.DOScale(defaultSize, 0.5f).SetEase(Ease.OutQuad);

        yield return tween.WaitForCompletion();
        isCoroutineRunning = false;
        GameManager.Instance.HideUI<ZoomUI>();
        GameManager.Instance.Player.IsZoom = 0;
    }

}
