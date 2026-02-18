using UnityEngine;

public abstract class AbstractItem : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
			onPickUp(other.gameObject);
	}

	public void onPickUp(GameObject player)    //метод, вызываемый при подборе аптечки
	{
		//if (player.GetComponent<Ammunition>().addAmmo(WeaponTypes.Machinegun, Ammo))
		//	Destroy(gameObject);
	}
}
