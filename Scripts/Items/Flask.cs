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
        if (player.GetComponent<Health>().changeHealth(Heal))
            Destroy(gameObject);
    }
}
