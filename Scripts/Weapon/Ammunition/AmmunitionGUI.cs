using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

[Serializable]
public class WeaponGUI    //структура, связывающая тип оружия и текстовое поле для отображения боезапаса
{
	public WeaponTypes weaponType;
	public TMP_Text text;
}

public class AmmunitionGUI : MonoBehaviour
{
	public Ammunition ammunition;    //ссылка на боезапас

	public List<WeaponGUI> weaponsList;    //список элементов интерфейса
	Dictionary<WeaponTypes, TMP_Text> weaponsDictionary;    //словарь элементов интерфейса

	public void listToDictionary()    //метод, преобразующий список в словарь
	{
		weaponsDictionary = new Dictionary<WeaponTypes, TMP_Text>();
		foreach (var weapon in weaponsList)
			if (weaponsDictionary.ContainsKey(weapon.weaponType) == false)
				weaponsDictionary.Add(weapon.weaponType, weapon.text);
	}

	private void Start() => listToDictionary();

	public void updateGUI()
	{
		foreach (KeyValuePair<WeaponTypes, int> kvp in ammunition.ammoDictionary)
			if(weaponsDictionary.ContainsKey(kvp.Key))
				weaponsDictionary[kvp.Key].text = kvp.Value.ToString();
	}
}