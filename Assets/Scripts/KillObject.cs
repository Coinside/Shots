using UnityEngine;
using System.Collections;

public class KillObject : MonoBehaviour {

	public GameObject objectToKill;
	public float timeToKill = 5.0f;
	// Use this for initialization
	void Start () {
		Destroy(objectToKill, timeToKill);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
