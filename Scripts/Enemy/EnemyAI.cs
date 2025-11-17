using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public GameObject flaskPrefab;
    public Health health;
    public Animator animator;

    public float attackRange = 2f;       // радиус атаки
    public int damage;

    public float viewRadius = 8f;        // радиус "зрения"
    public float chaseRadius = 12f;      // радиус, в пределах которого враг продолжает гнаться
    public float viewAngle = 90f;        // угол зрения
    public float patrolRadius = 5f;      // радиус патрулирования от точки спавна
    public float patrolWaitTime = 2f;    // время ожидания на точке патруля

    private Transform player;
    private NavMeshAgent agent;
    private bool isChasing = false;
    private Vector3 spawnPoint;
    private Vector3 patrolTarget;
    private float waitTimer;

	public LayerMask playerLayer;

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
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!isChasing)
        {
            // Проверяем игрока
			Collider[] cols = Physics.OverlapSphere(transform.position, viewRadius, playerLayer);     //проверка наличия игрока в радиусе обнаружения

			if (cols.Length > 0)                                   //если игрок попал в радиус обнаружения
			{
				//agent.SetDestination(cols[0].transform.position);

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

        if (health.currentHealth == 0)
        {
            death();
        }
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
}