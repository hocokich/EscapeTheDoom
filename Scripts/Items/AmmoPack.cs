  using UnityEngine;

public class AmmoPack : AbstractItem
{
    [Header("Ammo")]
    public int Ammo = 5;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            onPickUp(other.gameObject);
    }

    new public void onPickUp(GameObject player)    //метод, вызываемый при подборе аптечки
    {
        if (player.GetComponent<Ammunition>().addAmmo(WeaponTypes.Machinegun, Ammo))
            Destroy(gameObject);
    }
}
