using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseHover : MonoBehaviour
{
    RectTransform rectTransform;
    float targetWidth = 0f;
    bool mouseOver;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector3 endWidth = new Vector2(targetWidth, rectTransform.sizeDelta.y);
        rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, endWidth, 0.05f);
        if (targetWidth==0f && rectTransform.rect.width <= 5.0f)
        {
            rectTransform.sizeDelta = new Vector2(0.0f, rectTransform.sizeDelta.y);
        }
        //else if (rectTransform.localScale.x >= targetWidth-0.1f)
        //{
        //    rectTransform.sizeDelta = new Vector2(targetWidth, rectTransform.sizeDelta.y);
        //}
    }

    public void MouseOn(RectTransform _button)
    {
        if (isActiveAndEnabled)
        {
            targetWidth = _button.sizeDelta.x;
        }
    }

    public void OnClick()
    {
        targetWidth = 0f;
        rectTransform.sizeDelta = new Vector2(0.0f, rectTransform.sizeDelta.y);
    }

    public void MouseOff()
    {
        targetWidth = 0f;
    }
}
