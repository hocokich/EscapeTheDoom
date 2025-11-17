using UnityEngine.Events;
using UnityEngine;

public class Health : MonoBehaviour    //класс, отвечающий за здоровье персонажа
{
	[Range(1, 100)]
	[SerializeField] public int maximumHealth;    //максимально допустимое здоровье
	[Range(1, 100)]
	[SerializeField] public int currentHealth;    //текущее здоровье

	public UnityEvent<int, int> onHealthChange;    //ссылка на событие, срабатывающее при изменении текущего здоровья

	public UnityEvent spawnOnDeath;       //ссылка на обработчики события генерации предмета в позиции противника при его смерти
	public UnityEvent onDeath;                     //ссылка на обработчики события смерти
	public UnityEvent onHitTaken;                  //ссылка на обработчики события получения удара

	private void Start() => onHealthChange?.Invoke(currentHealth, maximumHealth);

	public bool changeHealth(int amount)    //метод, описывающий изменение текущего здоровья
	{
		if (currentHealth == maximumHealth)
			return false;

		currentHealth += amount;

		if (currentHealth > maximumHealth)
			currentHealth = maximumHealth;

		if (currentHealth < 0)
			currentHealth = 0;

		onHealthChange?.Invoke(currentHealth, maximumHealth);

		return true;
	}

	public void hpDecrease(float amount)    //метод, описывающий уменьшение текущего здоровья
	{
		if (currentHealth <= 0) return;

		onHitTaken?.Invoke();

		currentHealth = Mathf.FloorToInt(currentHealth - amount);

		if (currentHealth < 0)
			currentHealth = 0;

		onHealthChange?.Invoke(currentHealth, maximumHealth);

		if (currentHealth <= 0)
		{
			onDeath?.Invoke();
			spawnOnDeath?.Invoke();
		}
	}

	public void hpIncrease(float amount)
    {
		currentHealth = Mathf.FloorToInt(currentHealth + amount);

		onHealthChange?.Invoke(currentHealth, maximumHealth);

	}
}