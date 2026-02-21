using UnityEngine;

public class AmmoKitDialog : MonoBehaviour
{
	[Header("Ammo")]
	public int Ammo = 5;

	private void OnMouseDown()
	{
		//Ќаходим игрока и вписываем ему патроны
		GameObject.Find("Player").GetComponent<Player>().Ears -= 5;
		GameObject.Find("Player").GetComponent<Ammunition>().addAmmo(WeaponTypes.Machinegun, Ammo);

		//ѕередаем в диалог что сделка завершена успешно
		GameObject DialogCanvas = GameObject.Find("DialogCanvas");
		DialogCanvas.GetComponent<DialogTrader>().Dialog("TradeCompleted");
	}
}
