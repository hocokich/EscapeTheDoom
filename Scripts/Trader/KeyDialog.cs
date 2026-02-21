using UnityEngine;

public class KeyDialog : MonoBehaviour
{
	private void OnMouseDown()
	{
		//Находим игрока и вписываем ему ключи
		GameObject.Find("Player").GetComponent<Player>().Ears -= 5;
		GameObject.Find("Player").GetComponent<Player>().Keys++;

		//Передаем в диалог что сделка завершена успешно
		GameObject DialogCanvas = GameObject.Find("DialogCanvas");
		DialogCanvas.GetComponent<DialogTrader>().Dialog("TradeCompleted");
	}
}
