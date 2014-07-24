using UnityEngine;
using System.Collections;

public class CheckCollision : MonoBehaviour {
	private Light objL;
	private GameObject objLight;
	public string lightName = "Spotlight_1";
	private Camera mainCamera;  
	private Collider coll;
	private bool isOn = false;
	private bool isAwaitngOrder = false;
	private ClientOrderManager com;
	public float stoppingMagnitudeThreshold = 0.1f;
	public string cupTag;
	public float phaseTime = 3;
	public Color[] phaseColors;

	private int currentPhase = 0;
	private float lastUpdate = 0;


	// Use this for initialization
	void Start () {
		mainCamera = FindCamera();
		com = mainCamera.GetComponent<ClientOrderManager>();
		objLight = transform.FindChild(lightName).gameObject;

		objLight.light.color = phaseColors[0];
	}

	private Camera FindCamera () {
		
		return Camera.main;
	}

	// Update is called once per frame
	void Update () 
	{
		if(isAwaitngOrder)
		{
			if(lastUpdate == 0)
				lastUpdate = Time.realtimeSinceStartup;

			if(Time.realtimeSinceStartup - lastUpdate > phaseTime)
			{
				if(phaseColors.Length <= currentPhase + 1)
				{
					RegisterOrder(-1);
				}
				else
				{
					print ( "" + currentPhase);
					currentPhase++;
					objLight.light.color = phaseColors[currentPhase];
				}
				lastUpdate = 0;
			}
		}

		if(coll != null)
		{
			CupStates cupState = coll.rigidbody.gameObject.GetComponent<CupStates>();
			if(!cupState.IsValid())
			{
				isOn = false;
			}
		}

		if(isOn && coll == null)
		{
			isOn = false;
		}
	}

	private void RegisterOrder (int phase)
	{
		com.RegisterOrder(this.gameObject, phase);
		currentPhase = 0;
		objLight.light.color = phaseColors[0];
		isAwaitngOrder = false;
		lastUpdate = 0;
	}

	void OnTriggerStay (Collider  collider)
	{
		if(collider.gameObject.tag == cupTag)
		{
			CupStates cupState = collider.rigidbody.gameObject.GetComponent<CupStates>();
			if(/*collider.rigidbody.velocity.x == 0 &&
			   collider.rigidbody.velocity.y == 0 &&
			   collider.rigidbody.velocity.z == 0 &&*/
			   collider.rigidbody.velocity.magnitude <= stoppingMagnitudeThreshold &&
			   cupState.IsValid())
			{
				isOn = true;
				coll = collider;
				if(isAwaitngOrder)
					RegisterOrder(currentPhase);
			}
			else
			{
				isOn = false;
			}
		}
	}

	public void SetLightsOn (bool OnOff)
	{
		if(OnOff)
			objLight.light.intensity = 3.0f;
		else
			objLight.light.intensity = 0.0f;


	}

	public void SetAwatingOrder (bool isAwating)
	{
		isAwaitngOrder = isAwating;
	}

	public bool IsAwatingOrder ()
	{
		return isAwaitngOrder;
	}

	public bool IsOn ()
	{
		return isOn;
	}
}
