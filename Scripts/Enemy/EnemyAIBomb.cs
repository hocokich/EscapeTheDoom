using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

public class EnemyAIBomb : MonoBehaviour
{
	[Header("Loot")]
	public List<GameObject> Loot = new List<GameObject>();  //список типов орудий и их боезапас

	[Header("Prefab Explosion")]
	public GameObject exlosionPrefab;


	[Header("Health")]
	public Health health;

	[Header("Animators")]
	public Animator animator;

	[Header("Attack")]
	public float attackRange = 1f;       // радиус атаки
    public int damage;

	[Header("Navigation Attributes")]
	public float viewRadius = 8f;        // радиус "зрения"
    public float chaseRadius = 12f;      // радиус, в пределах которого враг продолжает гнаться
    public float viewAngle = 90f;        // угол зрения
    public float patrolRadius = 5f;      // радиус патрулирования от точки спавна
    public float patrolWaitTime = 2f;    // время ожидания на точке патруля

	[Header("Enemy of our enemy")]
	public LayerMask playerLayer;

	[Header("Sounds")]
	public Sounds Sounds;

    [Header("Material")]
	public Renderer enemyRenderer;

	private Transform player;
    private NavMeshAgent agent;
    private bool isChasing = false;
    private Vector3 spawnPoint;
    private Vector3 patrolTarget;
    private float waitTimer;

	void Start()
    {
		agent = GetComponent<NavMeshAgent>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        spawnPoint = transform.position;
        SetNewPatrolPoint();
	}
    void Update()
    {
        //if (health.currentHealth == 0) death();

		animator.SetFloat("speed", agent.speed);

        float distance = Vector3.Distance(transform.position, player.position);

        if (!isChasing)
        {
            // Проверяем игрока
            Collider[] cols = Physics.OverlapSphere(transform.position, viewRadius, playerLayer);     //проверка наличия игрока в радиусе обнаружения

            if (cols.Length > 0)                                   //если игрок попал в радиус обнаружения
            {
                agent.SetDestination(cols[0].transform.position);
                isChasing = true;
            }
            else Patrol();
        }

        if (isChasing)
        {
			// Звук преследования
			//Sounds.PlayClip(Sounds.clips[0]);

			agent.SetDestination(player.position);

            if (Vector3.Distance(transform.position, player.position) <= attackRange) //если дистанция до игрока меньше дистанции атаки
            {                                                                         //то пробуем атаковать
				agent.speed = 0f;                   //стопаем нашего врага, чтобы он атаковал
				animator.SetTrigger("attack");      //сзапускаем анимашку
			}

            // Если игрок слишком далеко — враг теряет интерес
            if (distance > chaseRadius)
            {
                isChasing = false;
                SetNewPatrolPoint();
            }
        }
	}

	void Patrol()
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
	void SetNewPatrolPoint()
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

	public void getDamage()
    {
		Sounds.PlayClip(Sounds.clips[2]);

		//замедляем врага если по нему попали
		agent.speed = 1.0f;

        //запускаем красное мигание
		StartCoroutine(Flash());
	}
	public void onExplode() //метод нанесения повреждений, вызывается из анимации атаки
	{
		// Находим все объекты с коллайдерами в радиусе атаки
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange * 1.75f);

		foreach (Collider hitCollider in hitColliders)
		{
			// Пытаемся получить компонент Health на объекте
			Health targetHealth = hitCollider.GetComponent<Health>();

			// Если компонент Health найден
			if (targetHealth != null)
			{
				// Игнорируем проверку на себя - наносим урон всем, включая себя
				Sounds.PlayClip(Sounds.clips[1]); //Звук попадания
				targetHealth.hpDecrease(damage); //уменьшение здоровья
			}
		}
	}

	public void death()
	{
		onExplode();

		agent.speed = 0f;

		Sounds.PlayClip(Sounds.clips[3]);

		animator.SetBool("dead", true);
	}
	public void onDeath()
	{
		//Эффект взрыва
		Instantiate(exlosionPrefab, transform.position, Quaternion.identity);

		addKillToPlayer();
		Destroy(gameObject);
		spawnItem();
	}

	void addKillToPlayer()
	{
		GameObject.Find("Player").GetComponent<Player>().killCount++;
	}
	void spawnItem()
    {
		Vector3 spawnPos = transform.position;		//Приподнимаем дроп (костыль)
		spawnPos.y = 0f;
		Vector3 spawnPosEar = spawnPos;
		spawnPosEar.z += 0.5f; //чуть чуть в бок
		spawnPosEar.y += 0.05f; //чуть чуть вверх

		try
		{
			//Спавним рандомный лут
			Instantiate(RandomItem(), spawnPos, Quaternion.identity);
		}
		catch
		{
			Debug.Log("Предмета нет");
		}
	}

	GameObject RandomItem()
	{
		int roll = Random.Range(0, 100);

		if (roll < 25) return null;
		if (roll < 75) return Loot[0];
		return Loot[1];

	}

	private IEnumerator Flash()
	{
		// Включаем свечение и устанавливаем HDR красный цвет
		enemyRenderer.material.EnableKeyword("_EMISSION");

		// Для HDR цвета используем Color.red с высокой интенсивностью
		// Например, Color.red * 0.1f для яркого свечения
		enemyRenderer.material.SetColor("_EmissionColor", Color.red * 0.2f);

		// Ждем немного
		yield return new WaitForSeconds(0.1f);

		// Плавно уменьшаем свечение
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
		// Полностью выключаем свечение
		enemyRenderer.material.SetColor("_EmissionColor", Color.black);
	}
    private IEnumerator backSpeed()
    {
		// Ждем немного
		yield return new WaitForSeconds(0.1f);

		// Плавно возвращаем обратный цвет
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