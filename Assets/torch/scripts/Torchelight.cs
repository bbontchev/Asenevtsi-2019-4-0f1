using UnityEngine;
using System.Collections;

public class Torchelight : MonoBehaviour {
	
	public GameObject TorchLight;
	public GameObject MainFlame;
	public GameObject BaseFlame;
	public GameObject Etincelles;
	public GameObject Fumee;
	public float MaxLightIntensity;
	public float IntensityLight;
	

	void Start () {
		TorchLight.GetComponent<Light>().intensity=IntensityLight;
		//depricated!
		//MainFlame.GetComponent<ParticleSystem>().emissionRate=IntensityLight*20f;
		//BaseFlame.GetComponent<ParticleSystem>().emissionRate=IntensityLight*15f;	
		//Etincelles.GetComponent<ParticleSystem>().emissionRate=IntensityLight*7f;
		//Fumee.GetComponent<ParticleSystem>().emissionRate=IntensityLight*12f;
		SetRateOverTime(MainFlame.GetComponent<ParticleSystem>(), IntensityLight*20f);
		SetRateOverTime(BaseFlame.GetComponent<ParticleSystem>(), IntensityLight*15f);	
		SetRateOverTime(Etincelles.GetComponent<ParticleSystem>(), IntensityLight*7f);
		SetRateOverTime(Fumee.GetComponent<ParticleSystem>(), IntensityLight*12f);
	}
	

	void Update () {
		if (IntensityLight<0) IntensityLight=0;
		if (IntensityLight>MaxLightIntensity) IntensityLight=MaxLightIntensity;		

		TorchLight.GetComponent<Light>().intensity=IntensityLight/2f+Mathf.Lerp(IntensityLight-0.1f,IntensityLight+0.1f,Mathf.Cos(Time.time*30));

		TorchLight.GetComponent<Light>().color=new Color(Mathf.Min(IntensityLight/1.5f,1f),Mathf.Min(IntensityLight/2f,1f),0f);
		//depricated!
		//MainFlame.GetComponent<ParticleSystem>().emissionRate=IntensityLight*20f;
		//BaseFlame.GetComponent<ParticleSystem>().emissionRate=IntensityLight*15f;
		//Etincelles.GetComponent<ParticleSystem>().emissionRate=IntensityLight*7f;
		//Fumee.GetComponent<ParticleSystem>().emissionRate=IntensityLight*12f;		
		SetRateOverTime(MainFlame.GetComponent<ParticleSystem>(), IntensityLight*20f);
		SetRateOverTime(BaseFlame.GetComponent<ParticleSystem>(), IntensityLight*15f);
		SetRateOverTime(Etincelles.GetComponent<ParticleSystem>(), IntensityLight*7f);
		SetRateOverTime(Fumee.GetComponent<ParticleSystem>(), IntensityLight*12f);		

	}

	void SetRateOverTime(ParticleSystem ps, float x) {
		ParticleSystem.EmissionModule module = ps.emission;
		ParticleSystem.MinMaxCurve curve = module.rateOverTime;
		curve.mode = ParticleSystemCurveMode.Constant;
		curve.constant = x; //  <--- here is where you put your constant value
		module.rateOverTime = curve;
	}
}