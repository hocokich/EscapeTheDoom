using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;

public class Ammunition : MonoBehaviour
{
	public List<WeaponAmmo> ammoList = new List<WeaponAmmo>();	//список типов орудий и их боезапас
	public Dictionary<WeaponTypes, int> ammoDictionary;			//словарь типов орудий и боезапаса

	public UnityEvent onAmmoChange; //событие, срабатывающее при изменении боезапаса

	private void Start()
	{
		if (GameManager.instance.level == 1) listToDictionary();
		onAmmoChange?.Invoke();
	}

	public bool checkAmmo(WeaponTypes type) //проверка, есть ли боеприпасы указанного типа
	{
		if (ammoDictionary.ContainsKey(type) == false)
			return false;
		if (ammoDictionary[type] < 1)
			return false;

		return true;
	}

	public bool getAmmo(WeaponTypes type) //получение боеприпаса указанного типа
	{
		if (ammoDictionary.ContainsKey(type) == false)
			return false;
		if (ammoDictionary[type] < 1)
			return false;

		ammoDictionary[type]--;
		onAmmoChange?.Invoke();

		return true;
	}

	public bool addAmmo(WeaponTypes type, int amount)
	{
		if(ammoDictionary.ContainsKey(type) == false)
			return false;

		ammoDictionary[type] += amount;
		onAmmoChange?.Invoke();
		return true;
	}

	public void listToDictionary()
	{
		ammoDictionary = new Dictionary<WeaponTypes, int>();

		foreach (var ammo in ammoList)
			if (ammoDictionary.ContainsKey(ammo.type) == false)
				ammoDictionary.Add(ammo.type, ammo.ammo);
	}

	IEnumerator WaitForGameManager()
	{
		// Ждем пока GameManager появится
		while (GameManager.instance == null)
		{
			Debug.Log("Ждем GameManager...");
			yield return null;
		}

		Debug.Log($"GameManager загружен, level = {GameManager.instance.level}");

		if (GameManager.instance.level == 1) listToDictionary();
		onAmmoChange?.Invoke();
	}
}