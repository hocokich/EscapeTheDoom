using TMPro;
using UnityEngine;

public class KillsTitle : MonoBehaviour
{
	[SerializeField] TMP_Text killCount;
	public Player player;
    void Update()
    {
		killCount.text = "Kills: " + player.killCount.ToString();
	}
}
