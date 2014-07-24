using UnityEngine;
using System.Collections;

public class DragRigidbodyMouse: MonoBehaviour {
	public float force = 1800.0f;
	private GameObject selectedObject;      // The object that is selected with the mouse
	private float fixedDistance;            // The set distance of the ray cast
	private Camera mainCamera;              // Cache the main camera
	private Vector3 forceVector;            // Vector to mouse position when not over table
	private RaycastHit hit;                 // Check all collisions from the raycast
	private Ray ray;                        // Stores the ray to test collision with the stones
	private Ray ray2;                       // Stores the ray to test collision with the stones
	
	private int launchBarSegmentLayerMask;             // Cache the layer mask for the stone table
	public int launchBarSegmentLayer;                  // Stores the layer that the stone table is on

	private int cupLayerMask;             // Cache the layer mask for the stone table
	public int cupLayer;                  // Stores the layer that the stone table is on

	/*public float x = 11.15841f;
	public float y = 12.2129f;
	public float z = 46.00331f;*/
	public float timeToDestroyInSeconds = 2;
	public float timeToDestroyShardsInSeconds = 1;

	private CupStates cupState;

	public AudioSource[] sounds;
	private AudioSource slideSound;
	private AudioSource breakingSound;
	private AudioSource clingSound;
	
	public string barTableObjectName;
	public string glassBreakingSurfacesTag = "glassBreaking";
	public string glassTag = "glass";

	public float stoppingMagnitudeThreshold = 0.1f;

	public GameObject TextureSpillPlane;
	public float xMancha = 11.15287f;
	public float yMancha = 12.1249f;
	public float zMancha = 49.30323f;
	public Color spillColor;

	private GameObject brokenCup;
	public GameObject broken;
	public float breakingVectorThreshold;
	public float spillingVectorThreshold;

	/// Start this instance.
	void Start() {

		TextureSpillPlane.gameObject.renderer.material.SetColor("_Color", spillColor);
		selectedObject = gameObject;
		cupState = selectedObject.GetComponent<CupStates>();

		brokenCup = Instantiate(broken, selectedObject.transform.position, selectedObject.transform.rotation) as GameObject;
		brokenCup.SetActive(false);

		// Cache the stone table layer mask
		launchBarSegmentLayerMask = 1 << launchBarSegmentLayer;
		cupLayerMask = 1 << cupLayer;
		
		// Find the main camera position
		mainCamera = FindCamera();

		// get the sounds from the parent object
		sounds = GetComponents<AudioSource>();
		slideSound = sounds[0];
		breakingSound = sounds[1];
		clingSound = sounds[2];
	}

	void LateUpdate () {

		if(selectedObject != null)
		{

			/*if(selectedObject.rigidbody.velocity.x == 0 &&
			   selectedObject.rigidbody.velocity.y == 0 &&
			   selectedObject.rigidbody.velocity.z == 0 )*/
			if(selectedObject.rigidbody.velocity.magnitude <= stoppingMagnitudeThreshold)
			{
				if(cupState.IsLaunched())
				{
					slideSound.Stop();
					DestroyObject(timeToDestroyInSeconds);
				}
				else if(cupState.IsDragging())
				{
					cupState.SetDragging(false);
				}
			}
		}

		// Cast a ray from the camera to the mouse position
		ray = mainCamera.ScreenPointToRay(Input.mousePosition);


		// Enable rotation if the stone is no longer dragged
		if (Input.GetButtonUp("Fire1")  && cupState != null && !cupState.IsLaunched() && cupState.IsValid() && cupState.IsDragging() && cupState.IsSelected()) {

			
			if (selectedObject != null) {
				selectedObject.rigidbody.useGravity = true;
				selectedObject.rigidbody.AddTorque(new Vector3(0,Random.Range(-1f,1f),0),ForceMode.Force);
			}
		}

		// Check that the mouse button is down
		if (Input.GetButtonDown("Fire1")){//&& !isLaunched && isValid) {

			RaycastHit hitted;
			if (Physics.Raycast(ray, out hitted, 100, cupLayerMask))
			{
				//if (Physics.Raycast(ray, out hitted))
				//{
				if (hitted.rigidbody != null && selectedObject == hitted.transform.gameObject)
					{
						//selectedObject = hitted.transform.gameObject;
						//cupState = selectedObject.GetComponent<CupStates>();
						cupState.SetSelected(true);
						if(cupState.IsValid() && !cupState.IsLaunched())
							selectedObject.rigidbody.useGravity = true;
					}
					else
					{
						if(selectedObject != null)
							cupState.SetSelected(false);

						//selectedObject = null;
						//cupState = null;
					}
				//}
			}
			else
			{
				if(selectedObject != null)
					cupState.SetSelected(false);
				//selectedObject = null;
				//cupState = null;
			}
		}
		
		// If the drag button is pressed and there is a selected object, drag the object
		if (Input.GetButton("Fire1") && cupState != null && !cupState.IsLaunched() && cupState.IsValid() && cupState.IsSelected()) {

			if (selectedObject != null) {
				// Drag the object
				DragObject(selectedObject, fixedDistance);
			}
		}
	}
	
	private void DragObject(GameObject go, float distance)
	{		
		// If ray collides with the stone table layer
		// (This is how the stone moves when the ray hits the table)
		if (Physics.Raycast(ray, out hit, 100, launchBarSegmentLayerMask))//, stoneTableMask))
		{
			cupState.SetDragging(true);
			// Get the direction of the point hit on the table
			forceVector = hit.point - go.transform.position;

			Vector3 newForceVector = new Vector3(forceVector.x, 0, forceVector.z);

			// Translate the selected object to the new vector using velocity
			go.rigidbody.velocity = newForceVector * Time.deltaTime * force;
		}
		/*else
			selectedObject.rigidbody.velocity = new Vector3(0,0,0);*/
	}
	
	// Returns the main camera
	private Camera FindCamera () {
		
		return Camera.main;
	}

	void OnTriggerExit(Collider collisionInfo) {
		if(collisionInfo.transform.name == "LaunchPlane")
		{
			cupState.SetLaunched(true);
			slideSound.Play();
		}
	}

	void OnCollisionExit(Collision collisionInfo) {
		if(collisionInfo.transform.name == barTableObjectName)
		{
			slideSound.Stop();
			selectedObject.rigidbody.freezeRotation = false;
			cupState.SetValid(false);
			cupState.SetOnTheTable(false);
		}
	}
	
	void OnCollisionEnter(Collision collisionInfo) {
		if(collisionInfo.transform.tag == glassBreakingSurfacesTag)
			Collision (false, true);

		if(collisionInfo.transform.tag == glassTag)
		{

			bool spill = (spillingVectorThreshold < selectedObject.rigidbody.velocity.magnitude);
		/*	if(breakingVectorThreshold > selectedObject.rigidbody.velocity.magnitude)
				Collision (cupState.IsValid() && spill, false);
			else
				Collision (cupState.IsValid() && spill, true);*/
			selectedObject.rigidbody.freezeRotation = true;
			if(breakingVectorThreshold > selectedObject.rigidbody.velocity.magnitude)
				Collision (spill, false);
			else
				Collision (spill, true);
		}
	}

	private void Collision (bool animateStain, bool breakObject)
	{
		if(cupState.IsOnTheTable() && animateStain )
		{
			selectedObject.rigidbody.freezeRotation = false;

			GameObject inst = Instantiate(TextureSpillPlane) as GameObject;
			Vector3 texturePosition = new Vector3(xMancha, yMancha, zMancha);
			inst.transform.position = texturePosition;

			Vector3 localPoint = inst.transform.InverseTransformPoint(selectedObject.transform.position);

			float xTexPos = ((localPoint.x / 10)-0.5f);
			float yTexPos = ((localPoint.z / 10)-0.5f);

			Vector4 TexCenter = inst.renderer.material.GetVector("_TexCenter");
			Vector4 vect = new Vector4(xTexPos, yTexPos, TexCenter.z, TexCenter.w);

			//inst.renderer.material.SetColor("_Color", spillColor);
			inst.renderer.material.SetVector("_TexCenter", vect);

		}

		if(breakObject)
			BreakCup();
		else
			clingSound.Play();

		DestroyObject(timeToDestroyInSeconds);
	}

	private void BreakCup ()
	{
		if(brokenCup != null)
		{
			breakingSound.Play();
			for (int i = 0; i < selectedObject.transform.childCount; ++i)
				selectedObject.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;

			//GameObject brokenCup = Instantiate(broken, selectedObject.transform.position, selectedObject.transform.rotation) as GameObject;
			brokenCup.transform.position = selectedObject.transform.position;
			brokenCup.transform.rotation = selectedObject.transform.rotation;

			brokenCup.SetActive(true);
			for (int i = 0; i < brokenCup.transform.childCount; ++i)
			{
				brokenCup.transform.GetChild(i).rigidbody.isKinematic = false;
				brokenCup.transform.GetChild(i).rigidbody.velocity = selectedObject.rigidbody.velocity;
			}
			
			Destroy(brokenCup, timeToDestroyShardsInSeconds);
		}
	}

	public void DestroyBroken ()
	{
		DestroyImmediate(brokenCup);
	}

	private void DestroyObject ()
	{
		cupState.SetValid(false);
		//DestroyObject(selectedObject, timeToDestroyInSeconds);
		DestroyObject(selectedObject);
		DestroyObject(brokenCup);
		CupListManager.RemoveCup(selectedObject);
	}

	private void DestroyObject (float time)
	{
		cupState.SetValid(false);
		//DestroyObject(selectedObject, timeToDestroyInSeconds);
		DestroyObject(selectedObject, time);
		DestroyObject(brokenCup, time);
		CupListManager.RemoveCup(selectedObject);
	}
}