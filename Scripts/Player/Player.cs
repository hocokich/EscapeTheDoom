using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Unity.VisualScripting;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    [Header("Sounds")]
    public Sounds Sound;
	public float stepInterval = 2.5f;
	private float stepTimer;

	[Header("Combat")]
    public Health health;
	public Ammunition ammunition;
	public GameObject fireballPrefab;
    public Transform firePoint;
    public GameObject machinegun;

    public bool isCasting = false;
	public bool canShoot = true; // ���� false � �������� ������

    int WeaponIndex;

    public UnityEvent onAmmoChange; //событие, срабатывающее при изменении боезапаса

    [Header("References")]
    public Animator animator;           // ����� ���������� �������
    public Transform cameraTransform;   // ����� ���������� �������

    public CharacterController cc;
	public Vector3 velocity;

	// --- ��� ����� ����� ---
	public Vector3 savedPosition;
	public Quaternion savedRotation;

    Player PreviusPlayer;

	void Awake()
	{
		//ammunition = GetComponent<Ammunition>();

		if (GameManager.instance != null)
		    PreviusPlayer = GameManager.instance.PreviusPlayer;
	}

	void Start()
    {
		if (GameManager.instance.level > 1 && GameManager.instance != null)
		{
			health.currentHealth = PreviusPlayer.health.currentHealth;

            ammunition.ammoDictionary = PreviusPlayer.ammunition.ammoDictionary;
		}

		WeaponIndex = 2;

        cc = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;

        GameManager.instance.player = gameObject;
    }

    void Update()
    {

        if (health.currentHealth == 0)
        {
            death();
        }

        //if (cameraTransform == null || animator == null) return;

        // --- установка находится ли игрок на земле ---
        bool grounded = cc.isGrounded;
        if (grounded && velocity.y < 0)
            velocity.y = -2f;

        // --- move ---
        float h = Input.GetAxisRaw("Horizontal"); // A/D
		float v = Input.GetAxisRaw("Vertical");   // W/S

		bool isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        if (isMoving)
        {
			stepTimer -= Time.deltaTime;

			if (stepTimer <= 0)
			{
				//Debug.Log("Двигается = " + isMoving);
				Sound.PlaySound(Sound.sounds[0]);
				stepTimer = stepInterval; // Сбрасываем таймер
			}

        }
		else
		{
			Sound.StopSound();
			//Debug.Log("Звука нет!");
			stepTimer = 0; // Сбрасываем таймер при остановке
		}

		// --- ����������� �������� ������������ ������ ---
		Vector3 camF = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camR = cameraTransform.right;
        Vector3 moveDir = (camF * v + camR * h);

        // --- ������� ��������� ������ �� ������� ---
        Vector3 lookDir = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), rotationSpeed * Time.deltaTime);
            transform.rotation = targetRot;
        }

        // --- �������� (�� ������� ���� ����) ---
        if (!isCasting)
        {
            Vector3 move = moveDir.normalized;
            if (move.magnitude >= 0.1f)
            {
                cc.Move(move * moveSpeed * Time.deltaTime);
            }
        }

        // --- ������ ---
        if (Input.GetButtonDown("Jump") && grounded && !isCasting)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        // --- ���������� ---
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

        // --- �������� �������� ---
        //animator.SetFloat("Forward", v);
        //animator.SetFloat("Right", h);

        if (Input.GetKeyDown("1"))
        {
            WeaponIndex = 1;
            machinegun.SetActive(false);
        }
        if (Input.GetKeyDown("2"))
        {
            WeaponIndex = 2;
            machinegun.SetActive(true);
        }

        // --- Если персонаж на земле и нажал кнопку выстрела, то произойдет попытка выстрела ---
        if (Input.GetMouseButtonDown(0) && grounded)
            TryShoot(WeaponIndex);
    }

    private void TryShoot(int WeaponIndex)
    {
        //if (!canShoot || animator == null) return;

        // ��������� ��������� ������� �� ����� ��������
        canShoot = false;

        // ��������� �������/�������
        savedPosition = transform.position;
        savedRotation = transform.rotation;

        switch (WeaponIndex)
        {
            case 1: //Fireball
                if(!isCasting && ammunition.checkAmmo(WeaponTypes.Fireball))
					spawnFireball();

                break;

            case 2: //Machinegun
				if (!isCasting && ammunition.checkAmmo(WeaponTypes.Machinegun))
					shoot();
                break;
        }

    }
    // ���� ����� ���������� Animation Event'�� (OnCastComplete) � �������, ��� ����� Animator
    public void OnCastComplete()
    {
        // Делается когда заканчивается каст
        transform.position = savedPosition;
        transform.rotation = savedRotation;

        isCasting = false;
        canShoot = true;
    }

    // �����������: ���� � ���� Apply Root Motion = true � �������� ������ root,
    // ����� ����������� root motion � ������������ ��������� transform �� animation:
    void OnAnimatorMove()
    {
        // ���� ������ � ��������� root motion ���������:
        // (��� �����������, ��� animation �� �������� transform � �����)
        if (animator != null)
        {
            // �� ��������� rootPosition / rootRotation:
            // ������ �� ������ � ��������� transform ��� ����.
        }
    }

    void spawnFireball()
    {
        isCasting = true;

        animator.SetTrigger("Cast");

        // spawning fireball � �������� ����: ������ ���� ���
        if (fireballPrefab != null && firePoint != null)
        {
            Vector3 dir = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            if (dir.sqrMagnitude < 1e-4f) dir = transform.forward;

            GameObject go = Instantiate(fireballPrefab, firePoint.position, Quaternion.LookRotation(dir));
            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.linearVelocity = dir * 12f;
            }
        }
    }
    void shoot()
    {
		onAmmoChange?.Invoke();
    }

    void death()
    {
        GameManager.instance.Lose();
    }

	private GameObject FindAmmunition(string name)
	{
		Ammunition[] ammunitions = FindObjectsOfType<Ammunition>();
		foreach (var a in ammunitions)
		{
			if (a.name == name)
				return a.gameObject;
			Debug.Log("Аммуниция найдена");

		}
		Debug.Log("Аммуниция не найден");
		return null;
	}
}