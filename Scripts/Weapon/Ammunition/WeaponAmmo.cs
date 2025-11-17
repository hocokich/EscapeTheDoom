using System;
using UnityEngine;

public enum WeaponTypes {Fireball, Machinegun }

[Serializable]
public class WeaponAmmo
{
	public WeaponTypes type;
	public int ammo;
}