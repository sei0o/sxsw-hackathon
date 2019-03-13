using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class wwwtest : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
	string url = "https://sechack365.nict.go.jp/common/imgs/sechack_logo_wb.png";
		WWW www = new WWW(url);
		yield return www;
		RawImage rawImage = GetComponent<RawImage>();
        rawImage.texture = www.textureNonReadable;

        //ピクセルサイズ等倍に
        rawImage.SetNativeSize();
	}
}
