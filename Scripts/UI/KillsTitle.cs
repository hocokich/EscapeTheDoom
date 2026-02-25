using TMPro;
using UnityEngine;

public class KillsTitle : MonoBehaviour
{
	[SerializeField] TMP_Text killCount;

	void Update()
    {
		int kc = GameObject.Find("Player").GetComponent<Player>().killCount;

		killCount.text = "Kills: " + kc.ToString();
	}
}
