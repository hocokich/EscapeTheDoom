using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour
{
	[Header("Loot")]
	public GameObject flaskPrefab;

	[Header("Components")]
	public Health health;
	public Animator animator;
	public Renderer enemyRenderer;

	[Header("Sounds")]
	public Sounds Sound;

	[Header("Enemy Settings")]
	public float attackRange = 2f;
	public int damage = 10;
	public float viewRadius = 8f;
	public float chaseRadius = 12f;
	public float viewAngle = 90f;
	public float patrolRadius = 5f;
	public float patrolWaitTime = 2f;
	public LayerMask playerLayer;

	// Экземпляр базового класса врага
	private EnemyAbstract enemyLogic;
	private NavMeshAgent agent;
	private Transform player;

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag("Player").transform;

		// Создаем конкретную реализацию базового врага
		enemyLogic = new MeleeEnemyLogic(
			attackRange,
			damage,
			viewRadius,
			chaseRadius,
			viewAngle,
			patrolRadius,
			patrolWaitTime
		);

		// Инициализируем
		enemyLogic.Initialize(player, agent, animator, health);
	}

	void Update()
	{
		enemyLogic.UpdateEnemy();
	}

	// Метод вызываемый при получении урона
	public void GetDamage()
	{
		agent.speed = 1.0f;
		StartCoroutine(Flash());
		enemyLogic.getDamage();
	}

	// Метод вызываемый из анимации атаки
	public void OnAttackAnimationEvent()
	{
		enemyLogic.OnAttack();
	}

	// Метод вызываемый при смерти
	public void Death()
	{
		enemyLogic.OnDeath();
	}

	private IEnumerator Flash()
	{
		enemyRenderer.material.EnableKeyword("_EMISSION");
		enemyRenderer.material.SetColor("_EmissionColor", Color.red * 0.2f);

		yield return new WaitForSeconds(0.1f);

		float timer = 0f;
		float duration = 0.5f;

		while (timer < duration)
		{
			timer += Time.deltaTime;
			float intensity = Mathf.Lerp(0.2f, 0f, timer / duration);
			enemyRenderer.material.SetColor("_EmissionColor", Color.red * intensity);
			agent.speed += 0.5f;
			yield return null;
		}

		enemyRenderer.material.SetColor("_EmissionColor", Color.black);
	}

	private IEnumerator BackSpeed()
	{
		yield return new WaitForSeconds(0.1f);

		float timer = 0f;
		float duration = 3.5f;

		while (timer < duration)
		{
			timer += Time.deltaTime;
			agent.speed += 1f * Time.deltaTime;
			yield return null;
		}
	}
}

// Конкретная реализация логики ближнего боя
public class MeleeEnemyLogic : EnemyAbstract
{
	private GameObject flaskPrefab;
	private Sounds sounds;
	private MonoBehaviour monoBehaviour;

	public MeleeEnemyLogic(
		float attackRange = 2f,
		int damage = 10,
		float viewRadius = 8f,
		float chaseRadius = 12f,
		float viewAngle = 90f,
		float patrolRadius = 5f,
		float patrolWaitTime = 2f)
		: base(attackRange, damage, viewRadius, chaseRadius, viewAngle, patrolRadius, patrolWaitTime)
	{
	}

	public override void OnAttack()
	{
		if (Vector3.Distance(agent.transform.position, player.position) <= attackRange)
		{
			Health playerHP = player.GetComponent<Health>();
			if (playerHP != null)
				playerHP.hpDecrease(damage);
		}

		// Восстанавливаем скорость через корутину
		if (agent.transform.TryGetComponent<MeleeEnemyAI>(out var enemyAI))
		{
			enemyAI.StartCoroutine(BackSpeedCoroutine());
		}
	}

	public override void OnDeath()
	{
		agent.isStopped = true;
		animator.SetBool("dead", true);

		// Отложенное уничтожение через корутину
		if (agent.transform.TryGetComponent<MeleeEnemyAI>(out var enemyAI))
		{
			enemyAI.StartCoroutine(DelayedDeath());
		}
	}

	public override void getDamage()
	{
		// Дополнительная логика при получении урона
		// Например, воспроизведение звука
		sounds.PlayClip(sounds.clips[0]);
	}

	public override void SpawnLoot()
	{
		if (flaskPrefab != null)
		{
			GameObject.Instantiate(flaskPrefab, agent.transform.position, Quaternion.identity);
		}
	}

	// Дополнительные методы для корутин
	private IEnumerator DelayedDeath()
	{
		yield return new WaitForSeconds(2f); // Ждем завершения анимации смерти
		SpawnLoot();
		GameObject.Destroy(agent.gameObject);
	}

	private IEnumerator BackSpeedCoroutine()
	{
		yield return new WaitForSeconds(0.1f);

		float timer = 0f;
		float duration = 3.5f;

		while (timer < duration)
		{
			timer += Time.deltaTime;
			agent.speed += 1f * Time.deltaTime;
			yield return null;
		}
	}
}