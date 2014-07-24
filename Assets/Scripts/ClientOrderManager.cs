using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientOrderManager : MonoBehaviour {

	public GameObject[] clientPositions;
	private List<int> orders;
	public int maxOrders = 3;
	public float minTimeBetweenOrders = 2;
	public float maxTimeBetweenOrders = 5;
	public string clientLightName = "Spotlight";

	private float lastExecution = 0;
	private float currentDelay = 0;

	// Use this for initialization
	void Start () {
		orders = new List<int>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(lastExecution == 0)
			lastExecution = Time.realtimeSinceStartup;

		if(orders.Count < maxOrders && (Time.realtimeSinceStartup - lastExecution) > currentDelay)
		{
			currentDelay = Random.Range(minTimeBetweenOrders, maxTimeBetweenOrders);
			lastExecution = Time.realtimeSinceStartup;
			int clientNumber = CreateOrderNumber();
			CreateOrder(clientNumber);
		}
	}

	private int CreateOrderNumber ()
	{
		int clientNumber = Random.Range(0, clientPositions.Length);
		for(int i = 0 ; i < orders.Count ; i++)
		{
			if(clientNumber == orders[i])
			{
				clientNumber = Random.Range(0, clientPositions.Length);
				i = -1;
			}
		}
		return clientNumber;
	}

	void CreateOrder(int clientNumber)
	{
		if(clientPositions.Length > 0 && clientPositions[clientNumber] != null)
		{
			CheckCollision scr = clientPositions[clientNumber].GetComponent<CheckCollision>();
			if(!scr.IsOn())
			{
				orders.Add(clientNumber);
				SetLights(true, clientNumber);
				SetAwatingOrder(true, clientNumber);
			}
		}
	}

	private void SetAwatingOrder (bool isAwating, int clientNumber)
	{
		CheckCollision scr = clientPositions[clientNumber].GetComponent<CheckCollision>();
		scr.SetAwatingOrder(isAwating);
	}

	private void SetLights (bool onOff, int clientNumber)
	{
		CheckCollision scr = clientPositions[clientNumber].GetComponent<CheckCollision>();
		scr.SetLightsOn(onOff);
	}

	public void RegisterOrder (GameObject client, int curretPhase)
	{
		for(int i = 0 ; i < clientPositions.Length ; i++)
		{
			if(client == clientPositions[i])
			{
				//SetAwatingOrder(false, i);
				SetLights(false, i);
				orders.Remove(i);
			}
		}
	}
}
