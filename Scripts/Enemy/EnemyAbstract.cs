using UnityEngine;
using UnityEngine.AI;

// Интерфейс для всех врагов
public interface IEnemy
{
	void Initialize(Transform playerTransform, NavMeshAgent navAgent, Animator anim, Health healthComponent);
	void UpdateEnemy();
	void OnAttack();
	void OnDeath();
	void getDamage();
	void SpawnLoot();
}

// Абстрактный базовый класс с общей логикой
public abstract class EnemyAbstract : IEnemy
{
	// Поля, которые будут у всех врагов
	protected Transform player;
	protected NavMeshAgent agent;
	protected Animator animator;
	protected Health health;
	protected Renderer enemyRenderer;

	// Настройки врага
	protected float attackRange;
	protected int damage;
	protected float viewRadius;
	protected float chaseRadius;
	protected float viewAngle;
	protected float patrolRadius;
	protected float patrolWaitTime;

	// Состояние
	protected bool isChasing;
	protected Vector3 spawnPoint;
	protected Vector3 patrolTarget;
	protected float waitTimer;
	protected LayerMask playerLayer;

	// Конструктор для базовых настроек
	protected EnemyAbstract(
		float attackRange = 2f,
		int damage = 10,
		float viewRadius = 8f,
		float chaseRadius = 12f,
		float viewAngle = 90f,
		float patrolRadius = 5f,
		float patrolWaitTime = 2f)
	{
		this.attackRange = attackRange;
		this.damage = damage;
		this.viewRadius = viewRadius;
		this.chaseRadius = chaseRadius;
		this.viewAngle = viewAngle;
		this.patrolRadius = patrolRadius;
		this.patrolWaitTime = patrolWaitTime;
	}

	// Реализация методов интерфейса
	public virtual void Initialize(Transform playerTransform, NavMeshAgent navAgent, Animator anim, Health healthComponent)
	{
		player = playerTransform;
		agent = navAgent;
		animator = anim;
		health = healthComponent;
		spawnPoint = agent.transform.position;
		SetNewPatrolPoint();
	}

	public virtual void UpdateEnemy()
	{
		if (health.currentHealth == 0)
		{
			OnDeath();
			return;
		}

		animator.SetFloat("speed", agent.velocity.magnitude);

		float distance = Vector3.Distance(agent.transform.position, player.position);

		if (!isChasing)
		{
			PatrolBehavior();

			// Проверка обнаружения игрока
			Collider[] cols = Physics.OverlapSphere(agent.transform.position, viewRadius, playerLayer);
			if (cols.Length > 0)
			{
				agent.SetDestination(cols[0].transform.position);
				isChasing = true;
			}
		}

		if (isChasing)
		{
			ChaseBehavior(distance);
		}
	}

	protected virtual void PatrolBehavior()
	{
		if (!agent.pathPending && agent.remainingDistance < 0.5f)
		{
			waitTimer += Time.deltaTime;
			if (waitTimer >= patrolWaitTime)
			{
				SetNewPatrolPoint();
				waitTimer = 0f;
			}
		}
	}

	protected virtual void ChaseBehavior(float distanceToPlayer)
	{
		agent.SetDestination(player.position);

		if (distanceToPlayer <= attackRange)
		{
			agent.isStopped = true;
			animator.SetTrigger("attack");
		}
		else
		{
			agent.isStopped = false;
		}

		if (distanceToPlayer > chaseRadius)
		{
			isChasing = false;
			SetNewPatrolPoint();
		}
	}

	protected virtual void SetNewPatrolPoint()
	{
		Vector3 randomDir = Random.insideUnitSphere * patrolRadius;
		randomDir += spawnPoint;
		NavMeshHit hit;

		if (NavMesh.SamplePosition(randomDir, out hit, patrolRadius, 1))
		{
			patrolTarget = hit.position;
			agent.SetDestination(patrolTarget);
		}
	}

	// Абстрактные методы для реализации в наследниках
	public abstract void OnAttack();
	public abstract void OnDeath();
	public abstract void getDamage();
	public abstract void SpawnLoot();
}