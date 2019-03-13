using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class move : MonoBehaviour, IPointerDownHandler {
	public AudioClip[] pianoSound;
	private AudioSource audioSource;
	public Color color;
	private Texture2D tex = null;
	WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
	float h,s,v;
	int size;
	// Use this for initialization
	IEnumerator Start () {
		audioSource = gameObject.GetComponent<AudioSource> ();
		tex = new Texture2D(1,1, TextureFormat.RGB24, false);
		string url = "https://sechack365.nict.go.jp/common/imgs/sechack_logo_wb.png";
		WWW www = new WWW(url);
		yield return www;
		RawImage rawImage = GetComponent<RawImage>();
        rawImage.texture = www.textureNonReadable;
				rawImage.SetNativeSize();
	}

	// Update is called once per frame
	void Update () {
		var dir = Vector3.zero;
				dir.x = Input.acceleration.x;
				dir.z = Input.acceleration.y;
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
	}
public void OnPointerDown (PointerEventData eventData) {
		Vector2 pos = Input.mousePosition;
		tex.ReadPixels(new Rect(pos.x, pos.y, 1,1), 0, 0);
		color = tex.GetPixel(0,0);
		Convert();
		SoundFilter();
		//audioSource.PlayOneShot (pianoSound[(int)Mathf.Round(v/10.0f)]);//Play sound
		int meido = (int)Mathf.Round(v/25.0f);
		if(meido==0){
			audioSource.PlayOneShot (pianoSound[7]);
			audioSource.PlayOneShot (pianoSound[2]);
			audioSource.PlayOneShot (pianoSound[4]);//Am
			Debug.Log("Am");
		}else if (meido==1){
			audioSource.PlayOneShot (pianoSound[9]);
			audioSource.PlayOneShot (pianoSound[5]);
			audioSource.PlayOneShot (pianoSound[7]);//F
			Debug.Log("F");
		}else if(meido==2){
			audioSource.PlayOneShot (pianoSound[8]);
			audioSource.PlayOneShot (pianoSound[6]);
			audioSource.PlayOneShot (pianoSound[3]);//G
			Debug.Log("G");
		}else{
			audioSource.PlayOneShot (pianoSound[2]);
			audioSource.PlayOneShot (pianoSound[4]);
			audioSource.PlayOneShot (pianoSound[6]);//C
			Debug.Log("C");
		}
	}
	void SoundFilter(){
		//GetComponent<AudioSource>().pitch = (v/100)*3;
		//GetComponent<AudioReverbFilter>().dryLevel = s/100*-10000;
	}
}
