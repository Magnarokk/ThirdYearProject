using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EfficiencyCalculator : MonoBehaviour {

	//wavelength averages range from 325nm to 1175nm with 50nm increments

	//step between all measured values in nanometers, used to calculate Input Power Density from Spectral Irradiance
	private int wavelengthDelta = 50;

	//in watts
	private float totalPowerIn = 0;

	//in watts
	private float totalPowerOut = 0;

	//in watts
	private float power = 0;

	//in watts
	private List<float> sectionPower; //unused



	/* LIGHT SOURCE SPECTRAL IRRADIANCE VALUES AND CORRECTIVE RATIOS */

	//Spectral Irradiance in watts per meter squared per nanometer (50nm increments between values) ==> WILL BE VARIABLE WITH SLIDER
	private float[] AM1p5SI = { //300nm to 2500nm
		0.2728f, 0.6436f, 1.2310f, 1.5562f, 1.5278f, 1.4932f, 1.4413f, 1.3454f, 1.2015f, 1.0643f,
		0.9766f, 0.9173f, 0.5389f, 0.5875f, 0.6950f, 0.5994f, 0.2428f, 0.3894f, 0.4518f, 0.4082f,
		0.2349f, 0.0003f, 0.0246f, 0.1159f, 0.2579f, 0.2527f, 0.2307f, 0.2111f, 0.1790f, 0.1048f,
		0.0030f, 0.0001f, 0.0036f, 0.0541f, 0.0630f, 0.0766f, 0.0895f, 0.0793f, 0.0749f, 0.0644f,
		0.0543f, 0.0417f, 0.0292f, 0.0143f
	};

	private float[] blackBodySI = { //300nm to 2000nm
		1.055f, 1.420f, 1.660f, 1.770f, 1.780f, 1.715f, 1.610f, 1.480f, 1.350f, 1.215f,
		1.085f, 0.975f, 0.870f, 0.780f, 0.700f, 0.620f, 0.550f, 0.490f, 0.440f, 0.390f,
		0.345f, 0.320f, 0.290f, 0.260f, 0.240f, 0.220f, 0.200f, 0.185f, 0.170f, 0.155f,
		0.145f, 0.130f, 0.115f, 0.105f
	};
	private float[] warmLEDSI = { //300nm to 850nm
		0.000f, 0.125f, 2.875f, 3.375f, 4.250f, 8.250f, 7.750f, 4.250f, 1.375f, 0.375f,
		0.125f
	};
	private float[] coolLEDSI = { //300nm to 800nm
		0.000f, 0.125f, 6.500f, 7.750f, 5.625f, 7.500f, 4.875f, 2.125f, 0.625f, 0.125f
	};
	private float[] broadBandFluoSI = { //300nm to 800nm
		0.000f, 1.500f, 6.925f, 6.750f, 7.325f, 7.025f, 5.500f, 3.600f, 1.500f, 0.500f
	};
	private float[] narrowTriBandFluoSI = { //300nm to 800nm
		0.000f, 0.250f, 4.070f, 3.900f, 7.100f, 5.250f, 8.700f, 1.000f, 1.000f, 0.250f
	};
	private float[] coolWhiteFluoSI = { //300nm to 800nm
		0.000f, 0.500f, 4.725f, 3.750f, 5.075f, 9.425f, 5.750f, 1.250f, 0.500f, 0.250f
	};
		
	//corrective ratio to adjust average power input of each light source to that of AM1.5
	private float blackBodyCorrectiveRatio = 0;
	private float warmLEDCorrectiveRatio = 0;
	private float coolLEDCorrectiveRatio = 0;
	private float broadBandFluoCorrectiveRatio = 0;
	private float narrowTriBandFluoCorrectiveRatio = 0;
	private float coolWhiteFluoCorrectiveRatio = 0;



	/* PHOTOVOLTAIC CELL EXTERNAL QUANTUM EFFICIENCIES AND SIGNIFICANT VALUES */

	//EQE percentages for specific frequency intervals (50nm increments between values)
	private float[] solexelEQE = {0.31f, 0.775f, 0.95f, 0.98f, 0.995f, 1.000f, 0.995f, 0.99f, 0.985f, 0.975f, 0.97f, 0.965f, 0.945f, 0.90f, 0.755f, 0.485f, 0.215f, 0.050f}; //300nm to 1200nm

	private float[] firstSolarEQE = {0.38f, 0.81f, 0.87f, 0.895f, 0.915f, 0.925f, 0.925f, 0.92f, 0.915f, 0.9f, 0.87f, 0.435f, 0.01f}; //300nm to 950nm

	private float[] fujikuraEQE = {0, 0.24f, 0.72f, 0.98f, 0.945f, 0.865f, 0.835f, 0.815f, 0.76f, 0.65f, 0.46f, 0.28f, 0.075f, 0.005f}; //300nm to 1000nm

	private float[] amorphAISTEQE = {0.12f, 0.5025f, 0.8575f, 0.9625f, 0.9875f, 0.9675f, 0.8775f, 0.6525f, 0.2975f, 0.055f}; //300nm to 800nm

	private float[] fhgIseConcentrTopEQE = {0.225f, 0.4975f, 0.7775f, 0.88f, 0.92f, 0.9175f, 0.885f, 0.605f, 0.175f}; //300nm to 750nm
	private float[] fhgIseConcentrBotEQE = {0.000f, 0.000f, 0.000f, 0.000f, 0.0025f, 0.0075f, 0.030f, 0.310f, 0.750f, 0.9325f, 0.9325f, 0.915f, 0.8875f, 0.5125f, 0.0775f, 0.0025f}; //300nm to 1100nm
	private float[] fhgIseConcentrEQE = new float[16]; //300nm to 1100nm, combination of the 2 previous, done in Start method

	private float[] fhgIseWaferTopEQE = {}; //300nm to
	private float[] fhgIseWaferMidEQE = {}; //300nm to
	private float[] fhgIseWaferBotEQE = {}; //300nm to
	private float[] fhgIseWaferEQE = new float[0]; //300nm to

	//actual efficiency for AM1.5
	private float solexelEff = 0.212f;
	private float firstSolarEff = 0.210f;
	private float fujikuraEff = 0.100f;
	private float amorphAISTEff = 0.102f;
	private float fhgIseConcentrEff = 0.352f;

	//surface area of cell in meters squared (m^2) ==> WILL BE VARIABLE WITH SLIDER
	private float solexelArea = 0.02397f;
	private float firstSolarArea = 0.00010623f;
	private float fujikuraArea = 0.002419f;
	private float amorphAISTArea = 1.001e-4f;
	private float fhgIseConcentrArea = 0.0002f; //COULDN'T FIND ACTUAL VALUE, PLACEHOLDER.

	//corrective ratio to adjust average EQE efficiency to actual efficiency of cell, for AM1.5 from 300nm to 2500nm
	private float solexelCorrectiveRatio = 0.28f;
	private float firstSolarCorrectiveRatio = 0.355f;
	private float fujikuraCorrectiveRatio = 0.1995f;
	private float amorphAISTCorrectiveRatio = 0.2403f;
	private float fhgIseConcentrCorrectiveRatio = 0.5485f;

	//create List containing PVCell objects and light source objects, and all their properties
	public List<PVCellData> PVCells;
	public List<LightSourceData> LightSources;

	//initializing Light Source datasets
	public LightSourceData AM1p5;
	public LightSourceData blackBody;
	public LightSourceData warmLED;
	public LightSourceData coolLED;
	public LightSourceData broadBandFluo;
	public LightSourceData narrowTriBandFluo;
	public LightSourceData coolWhiteFluo;

	//initializing PV Cell datasets
	public PVCellData solexel;
	public PVCellData firstSolar;
	public PVCellData fujikura;
	public PVCellData amorphAIST;
	public PVCellData fhgIseConcentr;

	//initializing the references to the dropdown menus
	public Dropdown PVCellDropdown;
	public Dropdown LightSourceDropdown;

	void Start () {

		/* ADDING PV CELL AND LIGHT SOURCE DATA TO LIST */

		PVCells.Add (solexel);
		PVCells.Add (firstSolar);
		PVCells.Add (fujikura);
		PVCells.Add (amorphAIST);
		PVCells.Add (fhgIseConcentr);

		LightSources.Add (AM1p5);
		LightSources.Add (blackBody);
		LightSources.Add (warmLED);
		LightSources.Add (coolLED);
		LightSources.Add (broadBandFluo);
		LightSources.Add (narrowTriBandFluo);
		LightSources.Add (coolWhiteFluo);

		List<string> PVCellNames = new List<string> (); 
		List<string> LightSourceNames = new List<string> (); 

		foreach (PVCellData pv in PVCells) {
			Debug.Log (PVCells.Find (PVCellData => PVCellData == pv).name + " online");
			PVCellNames.Add (pv.name);
		}

		foreach (LightSourceData ls in LightSources) {
			Debug.Log (LightSources.Find (LightSourceData => LightSourceData == ls).name + " online");
			LightSourceNames.Add (ls.name);
		}

		PVCellDropdown.AddOptions (PVCellNames);
		LightSourceDropdown.AddOptions (LightSourceNames);

		/* ASSIGNING SPECTRAL IRRADIANCE VALUES */

		AM1p5.AssignSI(AM1p5SI);
		blackBody.AssignSI (blackBodySI);
		warmLED.AssignSI (warmLEDSI);
		coolLED.AssignSI (coolLEDSI);
		broadBandFluo.AssignSI (broadBandFluoSI);
		narrowTriBandFluo.AssignSI (narrowTriBandFluoSI);
		coolWhiteFluo.AssignSI (coolWhiteFluoSI);



		/* EQE CONCATENATION OF MULTIJUNCTION CELLS */

		for (int i = 0; i < fhgIseConcentrBotEQE.Length; i++) { //Combination of Top and Bottom Concentrator Cells

			float EQETopVal;
			if (i >= fhgIseConcentrTopEQE.Length) //sets Top Concentrator EQE value to 0 if no value found at specified wavelength interval
				EQETopVal = 0;
			else
				EQETopVal = fhgIseConcentrTopEQE [i];

			fhgIseConcentrEQE [i] = fhgIseConcentrBotEQE [i] + EQETopVal; //sets combined values of Top and Bottom Concentrator Cells
		}



		/* ASSIGNING EXTERNAL QUANTUM EFFICIENCY VALUES */

		solexel.AssignEQE(solexelEQE);
		firstSolar.AssignEQE(firstSolarEQE);
		fujikura.AssignEQE(fujikuraEQE);
		amorphAIST.AssignEQE(amorphAISTEQE);
		fhgIseConcentr.AssignEQE(fhgIseConcentrEQE);



		/* CALCULATION OF SPLIT EFFICIENCIES */

		for (int i = 0; i < AM1p5SI.Length; i++) { //for every interval in the observed light source's SI

			float wavelength = 325 + 50 * i; //the wavelength starts at 325nm and increments in steps of 50

			totalPowerIn += AM1p5SI[i] * wavelengthDelta * amorphAISTArea; //add power in from interval to total power in

		}
		for (int i = 0; i < amorphAISTEQE.Length; i++) { //for every interval in the observed cell's EQE

			float wavelength = 325 + 50 * i; //the wavelength starts at 325nm and increments in steps of 50

			float SIVal;
			if (i >= AM1p5SI.Length) //sets Spectral Irradiation value to 0 if no value found at specified wavelength interval
				SIVal = 0;
			else
				SIVal = AM1p5SI [i];

			totalPowerOut += SIVal * wavelengthDelta * amorphAISTArea * amorphAISTEQE [i] * amorphAISTCorrectiveRatio; //add power out from interval to total power out
		}
			
		Debug.Log("Total Power OUT = " + totalPowerOut);
		Debug.Log("Total Power IN  = " + totalPowerIn);
		Debug.Log ("Efficiency = " + (100 * totalPowerOut / totalPowerIn) + "%");
	}

	void Update () {
	}

}


/*
float current = calculateCurrent (AM15SI[i]*wavelengthDelta, solexelArea, solexelEQE[i], wavelength);
sectionCurrent.Add(current);
totalCurrent += current;
Debug.Log("Section " + i + ": Current = " + current + ", Wavelength = " + wavelength);
*/

//power = calculatePower (AM15SI[i]*wavelengthDelta, solexelArea, solexelEQE[i]*solexelEffEQERatio);
//sectionPower.Add(power);
//Debug.Log("Section " + i + ": Power = " + power + ", Wavelength = " + wavelength);


//0.210f / 0.5427f; //0.5427 is the average of the firstSolarEffEQERatio
//0.212f / 0.791f; //0.791 is the average of the solexelEQE

/*
float calculateCurrent(float powerDensity, float surfaceArea, float EQE, float wavelength) {
	return (powerDensity * surfaceArea * EQE * 1.60e-19f) / (2.99e8f/(wavelength*1e-9f) * 6.63e-34f);
}

float calculatePower(float powerDensity, float surfaceArea, float Efficiency) {
	return Efficiency * powerDensity * surfaceArea;
}
*/