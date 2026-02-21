using TMPro;
using UnityEngine;

public class EarsCount : MonoBehaviour
{
	[SerializeField] TMP_Text earsCount;
	void Update()
	{
		Player player = GameObject.Find("Player").GetComponent<Player>();

		earsCount.text = player.Ears.ToString();
	}
}
