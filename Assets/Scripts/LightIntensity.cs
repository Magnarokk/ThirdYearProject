using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightIntensity : MonoBehaviour {

	private float multiplier;
    private Slider slider;

	// Use this for initialization
	void Start () {
        slider = gameObject.GetComponent<Slider>();
        slider.normalizedValue = 0.7f;
    }
	
	// Update is called once per frame
	void Update () {
        multiplier = slider.value;
        GameObject.Find("Lamp").GetComponent<Renderer>().material.color = new Color(multiplier+.1f, multiplier+.1f, multiplier+.1f);
		GameObject.Find("PV Cell").GetComponent<Renderer>().material.color = new Color(multiplier+.1f, multiplier+.1f, multiplier+.1f);
		GameObject.Find("BG").GetComponent<SpriteRenderer>().material.color = new Color(multiplier+.1f, multiplier+.1f, multiplier+.1f);
		GameObject.Find ("Spotlight").GetComponent<Light> ().intensity = multiplier * 50;
	}
}
