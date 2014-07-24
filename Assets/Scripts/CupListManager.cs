using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CupListManager : MonoBehaviour {

	private static List<GameObject> cups;

	// Use this for initialization
	void Start () {
		cups = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
		for(int i = 0 ; i < cups.Count ; i++)
		{
			if(cups[i] == null)
			{
				cups.RemoveAt(i);
				i = -1;
			}
		}
	}

	public static void AddCup (GameObject cup)
	{
		cups.Add(cup);
	}

	public static void RemoveCup (GameObject cup)
	{
		cups.Remove(cup);
	}

	public static int getCount ()
	{
		return cups.Count;
	}

	public static GameObject GetActiveCup ()
	{
		for(int i = 0 ; i < cups.Count ; i++)
		{
			if(cups[i] != null)
			{
				CupStates cs = cups[i].GetComponent<CupStates>();
				if(!cs.IsLaunched())
					return cups[i];
			}
		}
		return null;
	}

}
