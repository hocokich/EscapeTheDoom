using UnityEngine;

public class Ear : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
			onPickUp(other.gameObject);
	}
	new public void onPickUp(GameObject player)    //метод, вызываемый при подборе аптечки
	{
		player.GetComponent<Player>().Ears++;
		Destroy(gameObject);
		//¬кл звук подбора
		player.GetComponent<Player>().PickUpItemSound();
	}
}
