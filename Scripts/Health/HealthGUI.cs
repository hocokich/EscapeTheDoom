using UnityEngine;
using TMPro;

public class HealthGUI : MonoBehaviour
{
	[SerializeField] TMP_Text healthTect; 

	public void updateHealth(int currentHealth, int maximumHealth)
	{
		healthTect.text = "Çהמנמגüו: " + currentHealth.ToString() + "/" + maximumHealth.ToString();
	}
}