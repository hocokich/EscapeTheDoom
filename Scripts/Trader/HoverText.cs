using UnityEngine;

public class HoverText : MonoBehaviour
{
	public GameObject price;

	void OnMouseEnter()
	{
		price.SetActive(true);
	}

	void OnMouseExit()
	{
		price.SetActive(false);
	}
}