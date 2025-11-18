using UnityEngine;

[RequireComponent(typeof(TracerSystem))]    //для работы, класс требует компонент TracerSystem
[RequireComponent(typeof(MachinegunLogic))] //для работы, класс требует компонент MachinegunLogic
public class CMachineGun : AbstractWeapon
{
	TracerSystem tracerSystem;
	MachinegunLogic machinegumLogic;    //ссылка на обработчик выстрела

	private void Start()
	{
		tracerSystem = GetComponent<TracerSystem>();
		machinegumLogic = GetComponent<MachinegunLogic>(); //получение ссылки на компонент обработки выстрела
	}

	public override void fire(Ammunition ammunition) //метод, описывающий стрельбу
	{
		base.fire(ammunition);    //вызов метода, описанного в классе "абстрактное оружие"

		tracerSystem.CreateTracer(firePoint.position, firePoint.forward);
		machinegumLogic.shot(firePoint, damage);    //обработка выстрела
													//Здесь должен появится эффект
		Sound.PlaySound(Sound.sounds[0]);			// Звук выстрела
	}
	public override WeaponTypes getWeaponType()
	{
		return WeaponTypes.Machinegun;
	}
}