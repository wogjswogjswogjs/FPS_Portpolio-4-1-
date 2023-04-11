using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
           
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            
        }

    }
    
    /*public void FadeIn(float time)
    {
        this.isFadeOut = false;
        this.fadeTime1 = 0.0f;
        this.isFadeIn = true;
        StartCoroutine("DoFade", time);
    }

    public void FadeOut(float time)
    {
        this.isFadeIn = false;
        this.fadeTime1 = 0.0f;
        this.isFadeOut = true;
        StartCoroutine("DoFade", time);
    }
    
    public IEnumerator DoFade(float time)
    {
        while (fadeTime1 <= time)
        {
            if (this.isFadeIn == true)
            {
                this.fadeTime1 += Time.deltaTime;
                source.volume += Time.deltaTime/time;
            }
            else
            {
                this.fadeTime1 += Time.deltaTime;
                source.volume -= Time.deltaTime/time;
            }
            yield return null;
        }
    }*/
}
