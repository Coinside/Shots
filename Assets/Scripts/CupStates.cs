using UnityEngine;
using System.Collections;

public class CupStates : MonoBehaviour {

	private bool isLaunched = false;
	private bool isDragging = false;
	private bool isValid = true;
	private bool isSelected = false;
	private bool onTheTable = true;


	// Use this for initialization
	void Start () {
		isLaunched = false;
		isDragging = false;
		isValid = true;
		isSelected = false;
		onTheTable = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool IsOnTheTable()
	{
		return onTheTable;
	}

	public bool IsSelected()
	{
		return isSelected;
	}

	public bool IsValid()
	{
		return isValid;
	}

	public bool IsDragging()
	{
		return isDragging;
	}

	public bool IsLaunched()
	{
		return isLaunched;
	}

	public void SetValid (bool valid)
	{
		isValid = valid;
	}

	public void SetDragging (bool dragging)
	{
		isDragging = dragging;
	}

	public void SetLaunched (bool launched)
	{
		isLaunched = launched;
	}

	public void SetSelected (bool selected)
	{
		isSelected = selected;
	}

	public void SetOnTheTable (bool isOnTheTable)
	{
		onTheTable = isOnTheTable;
	}
}
