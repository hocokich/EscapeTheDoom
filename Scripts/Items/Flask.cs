  using UnityEngine;

public class Flask : AbstractItem
{
    [Header("Heal")]
    public int Heal = 5;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            onPickUp(other.gameObject);
    }

    new public void onPickUp(GameObject player)    //метод, вызываемый при подборе аптечки
    {
        player.GetComponent<Health>().hpIncrease(Heal);

        Destroy(gameObject);

		//¬кл звук подбора
		player.GetComponent<Player>().PickUpItemSound();
	}
}
