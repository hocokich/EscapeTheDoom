using UnityEngine;
using UnityEngine.Events;

public class WeaponCam : MonoBehaviour
{
	public UnityEvent OnShoot;     //событие, срабатывающее при изменении боезапаса
	public UnityEvent OnCut;     //событие, срабатывающее при изменении оружия

	public UnityEvent OnWeaponChange;     //событие, срабатывающее при изменении оружия

	public void TurnBackSpeed() => OnShoot?.Invoke();
	public void WeaponChange() => OnWeaponChange?.Invoke();
	public void Cut() => OnCut?.Invoke();
}