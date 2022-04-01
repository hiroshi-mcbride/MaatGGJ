using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    float fadeTime = 6f;
    

    private void Awake()
    {
        EventManager.AddListener(EventType.END_GAME, StartFadeIn);
    }
    public void StartFadeIn()
    {
        StartCoroutine(Fader());
    }

    IEnumerator Fader()
    {
        Color c = GetComponent<Image>().color;
        float alpha = GetComponent<Image>().color.a;
        while (alpha < 1f)
        {
            c.a += 0.01f / fadeTime;
            GetComponent<Image>().color = c;
            yield return new WaitForSeconds(0.01f);
            alpha = c.a;
        }

        for (int i=0; i<transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        //transform.GetChild(0).gameObject.SetActive(true);
        //transform.GetChild(1).gameObject.SetActive(true);
        //transform.GetChild(2).gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GetComponent<Image>().color.a>=1.0f)
        {
            Application.Quit();
        }
    }
}
