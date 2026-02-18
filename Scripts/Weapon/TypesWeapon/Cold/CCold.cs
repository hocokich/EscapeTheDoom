using Unity.VisualScripting;
using UnityEngine;

//[RequireComponent(typeof(TracerSystem))]    //для работы, класс требует компонент TracerSystem
[RequireComponent(typeof(ColdLogic))] //для работы, класс требует компонент ColdLogic
public class CCold : AbstractWeapon
{
	//TracerSystem tracerSystem;
	ColdLogic ColdLogic;    //ссылка на обработчик выстрела

	private void Start()
	{
		//tracerSystem = GetComponent<TracerSystem>();
		ColdLogic = GetComponent<ColdLogic>(); //получение ссылки на компонент обработки выстрела
	}

	public override void attack(Ammunition ammunition) //метод, описывающий стрельбу
	{
		base.attack(ammunition);    //вызов метода, описанного в классе "абстрактное оружие"

		StartCoroutine(stopEffect());

		//tracerSystem.CreateTracer(firePoint.position, firePoint.forward);
		ColdLogic.cut(firePoint, damage);    //обработка атаки
													//Здесь должен появится эффект
		sounds.PlayClip(sounds.clips[0]);           //Звук атаки

	}
	public override WeaponTypes getWeaponType()
	{
		return WeaponTypes.Cold;
	}
}