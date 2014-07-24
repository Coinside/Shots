using UnityEngine;
using System.Collections;

public class ActivateCollider : MonoBehaviour {

	public bool status = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetButtonDown("Fire1"))
			status = true;

		if(Input.GetButtonUp("Fire1"))
			status = false;

		this.gameObject.GetComponent<MeshCollider>().enabled = status;
	}
}
