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
    }

    public void MouseOn(RectTransform _button)
    {
        targetWidth = _button.sizeDelta.x;
        Debug.Log("yes");
    }

    public void MouseOff()
    {
        targetWidth = 0f;
    }
}
