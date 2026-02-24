using UnityEngine;

public class Key : AbstractItem
{
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
			onPickUp(other.gameObject);
	}

	new public void onPickUp(GameObject player)    //метод, вызываемый при подборе аптечки
	{
		player.GetComponent<Player>().Keys++;
			Destroy(gameObject);
		//¬кл звук подбора
		player.GetComponent<Player>().PickUpItemSound();
	}
}
