using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionEffect : MonoBehaviour
{
	public float effectDuration = 1;    //длительность эффекта
	public GameObject destructionEffect;    //ссылка на шаблон эффекта попадания цели

	public void Start()  //метод, описывающий процесс уничтожения снаряда
	{
		Destroy(gameObject, effectDuration); //уничтожение эффекта через назначенное время
	}
}
