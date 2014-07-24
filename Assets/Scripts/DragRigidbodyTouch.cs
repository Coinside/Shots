using UnityEngine;
using System.Collections;

public class DragRigidbodyTouch: MonoBehaviour {
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

	private CupStates cupState;

	public AudioSource[] sounds;
	private AudioSource noise1;
	private AudioSource noise2;
	
	public string barTableObjectName;
	public string glassBreakingSurfacesTag = "glassBreaking";
	public string glassTag = "glass";

	public GameObject TextureSpillPlane;
	public float xMancha = 11.15287f;
	public float yMancha = 12.1249f;
	public float zMancha = 49.30323f;
	/// Start this instance.
	void Start() {
		selectedObject = gameObject;
		cupState = selectedObject.GetComponent<CupStates>();

		// Cache the stone table layer mask
		launchBarSegmentLayerMask = 1 << launchBarSegmentLayer;
		cupLayerMask = 1 << cupLayer;
		
		// Find the main camera position
		mainCamera = FindCamera();

		// get the sounds from the parent object
		sounds = GetComponents<AudioSource>();
		noise1 = sounds[0];
		noise2 = sounds[1];
	}

	void LateUpdate () {

		if(selectedObject != null)
		{
			if(selectedObject.rigidbody.velocity.x == 0 &&
			   selectedObject.rigidbody.velocity.y == 0 &&
			   selectedObject.rigidbody.velocity.z == 0 )
			   {
				if(cupState.IsLaunched())
				{
					noise1.Stop();
					DestroyObject();
				}
				else if(cupState.IsDragging())
				{
					cupState.SetDragging(false);
				}
			}
		}

		// Cast a ray from the camera to the mouse position
		if(Input.touchCount == 1)
			ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);


		// Enable rotation if the stone is no longer dragged
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended  && cupState != null && !cupState.IsLaunched() && cupState.IsValid() && cupState.IsDragging()) {

			
			if (selectedObject != null) {
				selectedObject.rigidbody.useGravity = true;
				selectedObject.rigidbody.AddTorque(new Vector3(0,Random.Range(-1f,1f),0),ForceMode.Force);
			}
		}

		// Check that the mouse button is down
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began){//&& !isLaunched && isValid) {

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
		if (Input.touchCount == 1 && 
		    (Input.GetTouch(0).phase == TouchPhase.Moved || 
		 		Input.GetTouch(0).phase == TouchPhase.Stationary) && 
		    	cupState != null && !cupState.IsLaunched() && cupState.IsValid()) {

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
		else
			selectedObject.rigidbody.velocity = new Vector3(0,0,0);
	}
	
	// Returns the main camera
	private Camera FindCamera () {
		
		return Camera.main;
	}

	void OnTriggerExit(Collider collisionInfo) {
		if(collisionInfo.transform.name == "LaunchPlane")
		{
			cupState.SetLaunched(true);
			noise1.Play();
		}
	}
	
	void OnCollisionExit(Collision collisionInfo) {
		if(collisionInfo.transform.name == barTableObjectName)
		{
			noise1.Stop();
			selectedObject.rigidbody.freezeRotation = false;
			cupState.SetValid(false);
		}
	}
	
	void OnCollisionEnter(Collision collisionInfo) {
		if(collisionInfo.transform.tag == glassBreakingSurfacesTag)
			Collision (false);
		
		if(collisionInfo.transform.tag == glassTag)
		{
			print ("" + cupState.IsValid());
			Collision (cupState.IsValid());
		}
	}
	
	private void Collision (bool animateStain)
	{
		noise2.Play();
		selectedObject.rigidbody.freezeRotation = false;
		
		if(cupState.IsOnTheTable() && animateStain )
		{
			GameObject inst = Instantiate(TextureSpillPlane) as GameObject;
			Vector3 texturePosition = new Vector3(xMancha, yMancha, zMancha);
			inst.transform.position = texturePosition;
			
			Vector3 localPoint = inst.transform.InverseTransformPoint(selectedObject.transform.position);
			
			float xTexPos = ((localPoint.x / 10)-0.5f);
			float yTexPos = ((localPoint.z / 10)-0.5f);
			
			Vector4 TexCenter = inst.renderer.material.GetVector("_TexCenter");
			Vector4 vect = new Vector4(xTexPos, yTexPos, TexCenter.z, TexCenter.w);
			
			inst.renderer.material.SetVector("_TexCenter", vect);
		}
		cupState.SetValid(false);
		DestroyObject();
	}

	private void DestroyObject ()
	{
		cupState.SetValid(false);
		print ("Destroi");
		DestroyObject(selectedObject, timeToDestroyInSeconds);
		CupListManager.RemoveCup(selectedObject);
	}
}