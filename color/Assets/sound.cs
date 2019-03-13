using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class sound : MonoBehaviour {
	public AudioClip pianoSound;
	private AudioSource audioSource;
	public Color color;
	private Texture2D tex = null;
	public GameObject iro;
	WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
	float h,s,v;
	int size;
	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource> ();
		tex = new Texture2D(1,1, TextureFormat.RGB24, false);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
				Vector2 pos = Input.mousePosition;
				tex.ReadPixels(new Rect(pos.x, pos.y, 1,1), 0, 0);
				color = tex.GetPixel(0,0);
				iro.GetComponent<Renderer>().material.color = color;
				Convert();
				SoundFilter();
				audioSource.PlayOneShot (pianoSound);//Play sound
		}
	}
	//RGB→HSV
	void Convert (){
		float r = (color.r/1)*255;
		float g = (color.g/1)*255;
		float b=(color.b/1)*255;
		float min = Math.Min(Math.Min(r, g), b);
    float max = Math.Max(Math.Max(r, g), b);
		h = max - min;
    if (0.0f < h){
      if (max == r){
        h = 60.0f*((g - b) / h);
        if (h < 0.0f)
          {
            h += 360.0f;
          }
      }else if (max == g){
        h = 60.0f*((b* - r) / h)+120.0f;
      }
      else if (max==b){
        h = 60.0f*((r - g) / h)+240.0f;
      }else{
				h = 0.0f;
			}
    }
    s = ((max-min)/max)*100;
    v = ((max)/255)*100;
		//Debug.Log("HSV:"+h+","+s+","+v);
	}
	void SoundFilter(){
		GetComponent<AudioReverbFilter>().dryLevel = s/100*-10000;
		Debug.Log(s*100-10000);

		//GetComponent<AudioSource>().pitch = (v/100)*6-3;

	}
}
