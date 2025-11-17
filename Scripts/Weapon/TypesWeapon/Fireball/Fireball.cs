using UnityEngine;

public class Fireball : AbstractWeapon
{
    //public float lifeTime = 3f;

    void Start()
    {
        //Destroy(gameObject, lifeTime);
        //Destroy(gameObject);
    }

    public override WeaponTypes getWeaponType()
    {
        return WeaponTypes.Fireball;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject); // убиваем врага
            Destroy(gameObject);       // уничтожаем огонь
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject); // при ударе о стену пропадает
        }
    }
}