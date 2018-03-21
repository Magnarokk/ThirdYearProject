using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightIntensity : MonoBehaviour {

	private float multiplier;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		multiplier = gameObject.GetComponent<Slider>().value;
		GameObject.Find("Lamp").GetComponent<Renderer>().material.color = new Color(multiplier+.1f, multiplier+.1f, multiplier+.1f);
		GameObject.Find("PV Cell").GetComponent<Renderer>().material.color = new Color(multiplier+.1f, multiplier+.1f, multiplier+.1f);
		GameObject.Find("Background").GetComponent<Renderer>().material.color = new Color(multiplier+.1f, multiplier+.1f, multiplier+.1f);
		GameObject.Find ("Spotlight").GetComponent<Light> ().intensity = multiplier * 100;
	}
}
