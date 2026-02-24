using TMPro;
using UnityEngine;

public class DialogTrader : MonoBehaviour
{
	public GameObject QuestionTitle;
	public GameObject TradeItems;

	public GameObject ButtonYES;
	public GameObject ButtonNO;

	Player player;
	string question;

	void Awake()
	{
		player = GameObject.Find("Player").GetComponent<Player>();

		GameObject.Find("Player").GetComponent<Player>().Ears += 10;

		//Question = QuestionTitle.GetComponent<TextMeshPro>();
		TradeItems.SetActive(false);

		ButtonYES.SetActive(false);
		ButtonNO.SetActive(false);
	}
	void Update()
	{
		QuestionTitle.GetComponent<TMP_Text>().text = question;
	}

	void Start()
	{
		if(player.Ears == 0)
		{
			question = "Тебе нечего мне предложить ...";
			NextLvl();
			return;
		}
		if (player.Ears > 0 && player.Ears < 5)
		{
			question = "Хоть что то нашёл, но маловато ...";
			NextLvl();
			return;
		}
		if (player.Ears >= 5)
		{
			question = "Ого, не плох. Давай поторгуем ?";
			ButtonYES.SetActive(true);
			ButtonNO.SetActive(true);
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
		GameManager.instance.NextLvlFromTrader();
		//Фейд и переход на новый уровень
	}
}