using TMPro;
using UnityEngine;
using System.Collections;
using NUnit.Framework;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class DialogTrader : MonoBehaviour
{
	public GameObject QuestionTitle;
	public GameObject TradeItems;

	public GameObject ButtonYES;
	public GameObject ButtonNO;

	Player player;
	string question;

	bool used = false;

	void Awake()
	{

		TradeItems.SetActive(false);

		ButtonYES.SetActive(false);
		ButtonNO.SetActive(false);
	}

	void Update()
	{
		player = GameObject.Find("Player").GetComponent<Player>();

		QuestionTitle.GetComponent<TMP_Text>().text = question;

		Questions();
	}

	public void Questions()
	{
		if (used) return;
		//GameObject.Find("Player").GetComponent<Player>().Ears += 10;

		if (player.Ears == 0)
		{
			question = "Тебе нечего мне предложить ...";
			NextLvl();
			used = true;

			return;
		}
		if (player.Ears > 0 && player.Ears < 5)
		{
			question = "Хоть что то нашёл, но маловато ...";
			NextLvl();
			used = true;

			return;
		}
		if (player.Ears >= 5)
		{
			question = "Ого, не плох. Давай поторгуем ?";
			ButtonYES.SetActive(true);
			ButtonNO.SetActive(true);
			used = true;

			return;
		}
	}
	public void Dialog(string answer)
	{
		//Поторгуем ?
		switch (answer)
		{
			case "yes": /*Открывается диалог дальше*/
				question = "Выбирай ...";

				ButtonYES.SetActive(false);
				ButtonNO.SetActive(false);

				TradeItems.SetActive(true);
				break;

			case "no":  /*Закрывается диалог дальше*/
				question = "Прощай, в следующий раз не отказывайся ...";

				ButtonYES.SetActive(false);
				ButtonNO.SetActive(false);

				NextLvl();
				break;

			case "TradeCompleted":
				TradeItems.SetActive(false);
				question = "Прощай, ещё увидимся ...";

				NextLvl();
				break;

			default: /*Не понял, че?*/
				question = "Не понял... Че?";
				break;
		}
	}

	public void NextLvl()
	{
		StartCoroutine(DelayedNextLevel());
	}
	IEnumerator DelayedNextLevel()
	{
		yield return new WaitForSeconds(2f); // ждем 2 секунды
		GameManager.instance.NextLvlFromTrader();
	}
}