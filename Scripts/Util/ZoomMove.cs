using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomMove : MonoBehaviour
{
    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private PlayerInput input;

    private int padding;

    private void Update()
    {
        float x = Input.mousePosition.x - (Screen.width / 2);
        float y = Input.mousePosition.y - (Screen.height / 2);
        rect.localPosition = new Vector2(x, y);

        float tmp_cursorPosX = rect.localPosition.x;
        float tmp_cursorPosY = rect.localPosition.y;

        float min_width = -Screen.width / 2;
        float max_width = Screen.width / 2;
        float min_height = -Screen.height / 2;
        float max_height = Screen.height / 2;

        //if (input.isZoom)
        //{
        //    padding = 250;
        //    tmp_cursorPosY = Mathf.Clamp(tmp_cursorPosY, min_height + padding, max_height - padding);
        //    padding = 500;
        //    tmp_cursorPosX = Mathf.Clamp(tmp_cursorPosX, min_width + padding, max_width - padding);
        //}
        //else
        //{
        //    padding = 200;
        //    tmp_cursorPosY = Mathf.Clamp(tmp_cursorPosY, min_height + padding, max_height - padding);
        //    tmp_cursorPosX = Mathf.Clamp(tmp_cursorPosX, min_width + padding, max_width - padding);
        //}
        rect.localPosition = new Vector2(tmp_cursorPosX, tmp_cursorPosY);
    }
}
