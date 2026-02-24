using UnityEngine;

public class HoverText : MonoBehaviour
{
	private bool isHovered = false;

	public GameObject price;

	void OnMouseEnter()
	{
		isHovered = true;
		price.SetActive(true);
	}

	void OnMouseExit()
	{
		isHovered = false;
		price.SetActive(false);
	}
}