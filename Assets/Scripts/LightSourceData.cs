using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LightSourceData : ScriptableObject{

	public string name;
	public float correctiveRatio;
	public float[] spectralIrradiance;

	public void AssignSI(float[] si) {
		spectralIrradiance = si;
	}
}
