using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 5f;
    public float RotationSpeed = 10f;
    public float JumpHeight = 1.5f;
    public float Gravity = -20f;
	private float _h;					// A/D - horizontal move
	private float _v;                   // W/S - vertical move
	public bool CanMove;
	public int Ears;

	[Header("Sounds")]
    public Sounds Sounds;

	[Header("Combat")]
    public Health Health;				//Здоровье
	public Ammunition Ammunition;		//Описана логика работы системы оружия
	public List<GameObject> Weapons;    //список типов орудий и их боезапас
	public UnityEvent OnShootChange;     //событие, срабатывающее при изменении боезапаса
	public UnityEvent OnCutChange;     //событие, срабатывающее при изменении боезапаса

	public int killCount;               //счетчик убийств
	public int Keys;					//Счетчик ключей

	public Animator AnimWeaponCam;      //анимация оружия
	public int CurWeaponIndex;			//Текущее выбранное оружие
	public int PrevWeaponIndex;          //Предыдущее выбранное оружие

    [Header("References")]
    public Transform CameraTransform;   // Местоположение главной камеры
	public GameObject Map;

    public CharacterController CC;
	public Vector3 Velocity;

    Player PreviusPlayer;               //Нужно для сохранение информации об предыдущих ресурсах и тп.

	void Awake()
	{
		if (GameManager.instance != null)
		    PreviusPlayer = GameManager.instance._PreviusPlayer;
	}

	void Start()
    {
		if (GameManager.instance.level != 1 && GameManager.instance != null)
		{
			Health.currentHealth = PreviusPlayer.Health.currentHealth;
            Ammunition.ammoDictionary = PreviusPlayer.Ammunition.ammoDictionary;
			killCount = PreviusPlayer.killCount;
			Ears = PreviusPlayer.Ears;
			Keys = PreviusPlayer.Keys;
		}

		CurWeaponIndex = 1;

        CC = GetComponent<CharacterController>();
        if (CameraTransform == null && Camera.main != null) CameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;

        GameManager.instance.player = gameObject;
	}

	void Update()
    {
        if (Health.currentHealth == 0) death();

        // --- установка находится ли игрок на земле ---
        if (CC.isGrounded && Velocity.y < 0)
            Velocity.y = -2f;

		inputPlayer();

		//_soundSteps();

		//управление поворотом персонажа
		CC.Move(_ccDirection(_h, _v) * MoveSpeed * Time.deltaTime);

        // --- гравитация ---

		if(CanMove)
		{
			Velocity.y += Gravity * Time.deltaTime;
			CC.Move(Velocity * Time.deltaTime);
		}
	}

    private void _tryAttack(int WeaponIndex)				//Попытка совершить атаку 
    {
        switch (WeaponIndex)
        {
            case 1: //Machinegun
				if (Ammunition.checkAmmo(WeaponTypes.Machinegun) && Weapons[1].GetComponent<CMachineGun>().canAttack)	//Если условия выполены, то
                {
					MoveSpeed = 2.5f;                       //замедляем игрока
					AnimWeaponCam.SetTrigger("shoot");		//включается анимация стрельбы
					shoot();
				}
			break;
			case 2: //knife
				if (Ammunition.checkAmmo(WeaponTypes.Cold) && Weapons[2].GetComponent<CCold>().canAttack)   //Если условия выполены, то
				{
					MoveSpeed = 2.5f;                       //замедляем игрока
					AnimWeaponCam.SetTrigger("cut");      //включается анимация стрельбы
					cut();
				}
				break;
		}
    }

	void shoot() => OnShootChange?.Invoke();
	void cut() => OnCutChange?.Invoke();
	void death() => GameManager.instance.Lose();
	Vector3 _ccDirection(float h, float v) //Определяем направление игрока и возвращаем его для перемещения 
    {
		Vector3 camF = Vector3.Scale(CameraTransform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 camR = CameraTransform.right;
		Vector3 moveDir = (camF * v + camR * h);

		Vector3 lookDir = Vector3.Scale(CameraTransform.forward, new Vector3(1, 0, 1)).normalized;
		if (lookDir.sqrMagnitude > 0.01f)
		{
			Quaternion targetRot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), RotationSpeed * Time.deltaTime);
			transform.rotation = targetRot;
		}
        return moveDir;
	}

	public void inputPlayer()                     //Управление вводом игрока
	{
		if (!CanMove) return;
		// --- перемещение ---
		_h = Input.GetAxisRaw("Horizontal"); // A/D
		_v = Input.GetAxisRaw("Vertical");   // W/S
											 // --- Прыжок ---
		if (Input.GetButtonDown("Jump") && CC.isGrounded)
		{
			Velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
		}

		//Смена оружия
		if (Input.GetKeyDown("1"))
		{
			if (CurWeaponIndex == 1) return; //если мы и так нажали то не воспроизводим анимацию

			AnimWeaponCam.SetTrigger("change");

			PrevWeaponIndex = CurWeaponIndex;   //записываем предыдущее
			CurWeaponIndex = 1;
		}
		if (Input.GetKeyDown("2"))
		{
			if (CurWeaponIndex == 2) return; //если мы и так нажали то не воспроизводим анимацию

			AnimWeaponCam.SetTrigger("change");

			PrevWeaponIndex = CurWeaponIndex;   //записываем предыдущее
			CurWeaponIndex = 2;
		}

		//Карта - показывается когда зажата M, исчезает когда отпущена
		Map.SetActive(Input.GetKey(KeyCode.M));

		// --- Если персонаж на земле и нажал кнопку выстрела, то произойдет попытка выстрела ---
		if (Input.GetMouseButton(0) && CC.isGrounded)
			_tryAttack(CurWeaponIndex);
	}
	//Управление звуками
	void _soundSteps()					   //Управление звуками шагов
	{
		bool isMoving = Mathf.Abs(_h) > 0.1f || Mathf.Abs(_v) > 0.1f;

		if (isMoving && !Sounds.isPlaying)
		{
			AnimWeaponCam.SetFloat("speed", 1);

			// Запускаем звук
			Sounds.PlayClip(Sounds.clips[0]);
		}
		if (!isMoving)
		{
			AnimWeaponCam.SetFloat("speed", 0);     // останавливаем анимацию оружия
			Sounds.StopSound();                     // Мгновенно останавливаем звук
		}
	}
	public void PickUpItemSound()
	{
		Sounds.PlayClip(Sounds.clips[1]);
	}
	public IEnumerator _backSpeed(float etalon)	//Медленное возвращение скорости игроку
	{
		// Ждем немного и начием возвращать
		yield return new WaitForSeconds(0.1f);

		// Плавно возвращаем обратно
		while (MoveSpeed <= etalon)
		{
			MoveSpeed += 0.5f * Time.deltaTime;

			yield return null;
		}
	}

	//Для анимаций
	public void OnShoot() => MoveSpeed = 5f;
	public void OnCut() => MoveSpeed = 5f;
	public void OnWeaponChange()
	{
		Weapons[PrevWeaponIndex].SetActive(false);

		Weapons[CurWeaponIndex].SetActive(true);
	}

	//private GameObject FindAmmunition(string name)
	//{
	//	Ammunition[] ammunitions = FindObjectsOfType<Ammunition>();
	//	foreach (var a in ammunitions)
	//	{
	//		if (a.name == name)
	//			return a.gameObject;
	//		Debug.Log("Аммуниция найдена");

	//	}
	//	Debug.Log("Аммуниция не найден");
	//	return null;
	//}
}