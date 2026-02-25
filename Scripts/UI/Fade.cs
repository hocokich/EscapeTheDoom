using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelFade : MonoBehaviour
{
	Animator anim;

	void Awake() => anim = GetComponent<Animator>();
	void Start() => FadeOff();

	public void FadeOn()
	{
		anim.SetTrigger("FadeOn");
	}
	public void FadeOff()
	{
		anim.SetTrigger("FadeOff");
	}

	public void OnFadeOff()
	{
		gameObject.SetActive(false);
	}
}