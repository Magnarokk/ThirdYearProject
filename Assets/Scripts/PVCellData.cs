using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PVCellData : ScriptableObject {

	public string name;
	public string type;
	public float efficiency;
	public float surfaceArea;
	public float correctiveRatio;
	public float[] EQE;

	public void AssignEQE(float[] eqe) {
		EQE = eqe;
	}
}
