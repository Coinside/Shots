using UnityEngine;
using System.Collections;

public class VerifyClick : MonoBehaviour {

	public GameObject objectToCreate;
	private GameObject inst;

	public float x = 11.7313f;
	public float y = 12.22292f;
	public float z = 46.15465f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began && guiTexture.HitTest(Input.GetTouch(0).position))
			CreateObject ();
	}

	void OnMouseDown() {
		CreateObject ();
	}


	private void CreateObject ()
	{
		GameObject currentUnlanchedCup = CupListManager.GetActiveCup();
		while(currentUnlanchedCup != null)
		{
			if(currentUnlanchedCup != null)
			{
				CupListManager.RemoveCup(currentUnlanchedCup);
				DragRigidbodyMouse drm = currentUnlanchedCup.GetComponent<DragRigidbodyMouse>();
				drm.DestroyBroken();
				DestroyImmediate(currentUnlanchedCup);
			}
			currentUnlanchedCup = CupListManager.GetActiveCup();
		}
		//if(CupListManager.getCount() == 0)
		//{
			animation.Play();
			inst = Instantiate(objectToCreate) as GameObject;
			Vector3 cupPosition = new Vector3(x, y, z);
			inst.transform.position = cupPosition;
			CupListManager.AddCup(inst);
		//}
	}

}
