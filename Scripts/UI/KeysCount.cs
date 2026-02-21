using TMPro;
using UnityEngine;

public class KeysCount : MonoBehaviour
{
	[SerializeField] TMP_Text keysCount;
	void Update()
	{
		Player player = GameObject.Find("Player").GetComponent<Player>();

		keysCount.text = player.Keys.ToString();
	}
}
