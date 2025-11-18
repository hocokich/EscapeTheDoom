using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class EnemyAI : MonoBehaviour
{
	[Header("Loot")]
	public GameObject flaskPrefab;

	[Header("Health")]
	public Health health;
	[Header("Animator")]
	public Animator animator;

	[Header("Attack")]
	public float attackRange = 2f;       // радиус атаки
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
	public Sounds Sound;

    [Header("Material")]
	public Renderer enemyRenderer;
	private Material originalMaterial;
	private Material currentMaterial;
	private Color originalColor;
	private MaterialPropertyBlock propertyBlock;
	private Coroutine flashCoroutine;

	private Transform player;
    private NavMeshAgent agent;
    private bool isChasing = false;
    private Vector3 spawnPoint;
    private Vector3 patrolTarget;
    private float waitTimer;


	void Start()
    {
		// Создаем уникальную копию материала для этого врага
		currentMaterial = enemyRenderer.material;          // Автоматически создает инстанс
        originalMaterial = enemyRenderer.sharedMaterial;    // Сохраняем ссылку на оригинальный материал (опционально)
		originalColor = enemyRenderer.material.color;

		agent = GetComponent<NavMeshAgent>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        spawnPoint = transform.position;
        SetNewPatrolPoint();
    }

    void Update()
    {
        if (health.currentHealth == 0) death();

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
            else
            {
                Patrol();
            }
        }

        if (isChasing)
        {
            agent.SetDestination(player.position);

            animator.SetTrigger("attack");

            // Если игрок слишком далеко — враг теряет интерес
            if (distance > chaseRadius)
            {
                isChasing = false;
                SetNewPatrolPoint();
            }
        }

		Debug.Log(agent.speed);
		//Если снизилась скорость
		if (agent.speed < 3.5f)
        {
            agent.speed += 0.5f * Time.deltaTime;
        }
	}

    public void hitTaken()
    {
        //замедляем после выстрела
        agent.speed = 1.0f;

		StartCoroutine(Flash());

		//Color color = new Color(1f, 0.5f, 0.5f, 1f);
		//currentMaterial.SetColor("_BaseColor", color);
	}

    public void dealDamage() //метод нанесения повреждений
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange) //если дистанция до игрока меньше дистанции атаки
        {
            Health playerHP = player.GetComponent<Health>(); //попытка получить ссылку на здоровье игрока
            if (playerHP != null)
                playerHP.hpDecrease(damage); //уменьшение здоровья игрока
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
				// Звук преследования
				Sound.PlaySound(Sound.sounds[0]);

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

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f)
        {
            if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, out RaycastHit hit, viewRadius))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void death()
    {
        Destroy(gameObject);
        spawnFlask();
    }

    void spawnFlask()
    {
        Instantiate(flaskPrefab, transform.position, Quaternion.identity);
    }

	private IEnumerator Flash()
	{
		// Меняем на красный
		enemyRenderer.material.color = Color.red;

		// Ждем немного
		yield return new WaitForSeconds(0.1f);

		// Плавно возвращаем обратный цвет
		float timer = 0f;
		float duration = 0.5f;

		while (timer < duration)
		{
			timer += Time.deltaTime;
			enemyRenderer.material.color = Color.Lerp(Color.red, originalColor, timer / duration);
			yield return null;
		}

		// Гарантируем точный цвет
		enemyRenderer.material.color = originalColor;
	}
}