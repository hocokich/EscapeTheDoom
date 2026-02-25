using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Trader : MonoBehaviour
{
	//public UnityEvent OnCutChange;     //событие, срабатывающее при изменении боезапаса
	//public UnityEvent OnDialog;     //событие, срабатывающее при изменении боезапаса

	public TMP_Text healthTxt;
	public TMP_Text ammoTxt;

	// Update is called once per frame
	void Update()
    {
        int ammo = GameObject.Find("GameManager").GetComponent<GameManager>()._PreviusPlayer.Ammunition.ammoDictionary[WeaponTypes.Machinegun];
        int health = GameObject.Find("GameManager").GetComponent<GameManager>()._PreviusPlayer.Health.currentHealth;

		healthTxt.text = health.ToString();
		ammoTxt.text = ammo.ToString();
	}
}
