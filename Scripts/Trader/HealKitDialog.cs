using UnityEngine;

public class HealKitDialog : MonoBehaviour
{
	[Header("Heal")]
	public int Heal = 5;

	private void OnMouseDown()
	{
		//Находим игрока и вписываем ему хп
		GameObject.Find("Player").GetComponent<Player>().Ears -= 5;
		GameObject.Find("Player").GetComponent<Health>().changeHealth(Heal);

		//Передаем в диалог что сделка завершена успешно
		GameObject DialogCanvas = GameObject.Find("DialogCanvas");
		DialogCanvas.GetComponent<DialogTrader>().Dialog("TradeCompleted");
	}
}
