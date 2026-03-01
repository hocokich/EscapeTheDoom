using UnityEngine;
using TMPro;

public class HP_changeTitle : MonoBehaviour
{
	public Animator animatorUi;
	public TMP_Text hpChangeTitle;

	private void Start()
	{
		var MaiCam = Camera.main;
		Canvas canvas = GetComponent<Canvas>();

		canvas.worldCamera = MaiCam;

		animatorUi = GetComponent<Animator>();
	}

	public void fade(int amount)
	{
		hpChangeTitle.text = "-" + amount.ToString();
		animatorUi.SetTrigger("fade");
	}
}
