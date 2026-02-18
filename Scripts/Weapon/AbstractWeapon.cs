using UnityEngine;
using System.Collections;

public abstract class AbstractWeapon : MonoBehaviour, IWeapon  //абстрактный класс "оружие", реализующий интерфейс "оружие"
{
	public Transform firePoint;        //точка, из которой происходит выстрел
	public float fireRate;             //частота выстрелов (может использоваться по разному)
	public float damage;               //урон, наносимый оружием (механика нанесения может отличаться)
	[HideInInspector]
	public bool canAttack = true;        //готово ли оружие к выстрелу
	public ParticleSystem weaponEffect; //ссылка на эффект выстрела
	public Sounds sounds;                //ссылка на звуки выстрела

	private void Awake()
	{
		if (weaponEffect == null) return; //если оружие не имеет эффекта выстрела -> выход

		var main = weaponEffect.main;
		main.duration = 1 / fireRate; //получение ссылки на основные параметры системы частиц
									  //установка длительности эффекта равной частоте выстрелов
	}

	public virtual void attack(Ammunition ammunition)    //метод стрельбы, который может быть переопределён в наследниках класса
	{
		if(ammunition.getAmmo(getWeaponType()) == false) return;
		canAttack = false;    //изменение состояние готовности оружия
		StartCoroutine(coolDown());    //запуск обновления состояния оружия
	}

	public abstract WeaponTypes getWeaponType();

	public IEnumerator stopEffect()    //метод вкл/отключения эффекта
	{
		weaponEffect.Play();
		yield return new WaitForSeconds(0.1f / fireRate);
		weaponEffect.Stop();
	}

	public IEnumerator coolDown()    //метод обновления состояния готовности оружия
	{
		yield return new WaitForSeconds(1 / fireRate);
		canAttack = true;
	}

	private void OnEnable()    //событие включения (enable = true) объекта
	{
		if (canAttack == false)    //если оружие не готово к стрельбе
			StartCoroutine(coolDown()); //запуск процесса обновления готовности оружия
	}

	public void attack()
	{
		throw new System.NotImplementedException();
	}
}