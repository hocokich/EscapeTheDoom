using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class SimpleFollowCamera : MonoBehaviour
{
	public Player player;
	public float sensitivity = 3f;  // Чувствительность поворота (мышь X)

    private Vector3 offset;         // Смещение камеры от игрока (из сцены)
    private Quaternion initRot;     // ТВОЙ исходный ракурс камеры (из сцены)
    private float yaw = 0f;         // Накопленный поворот вокруг Y

	void Start()
    {
        if (player == null)
        {
            Debug.LogError("SimpleFollowCamera: назначь target (игрока)!");
            return;
        }

		// Запоминаем ТВОЮ расстановку из сцены
		Vector3 playerPos = player.GetComponent<Transform>().position;
        offset = new Vector3(playerPos.x, playerPos.y + 0.6f, playerPos.z -0.5f);
		initRot = transform.rotation;

        Cursor.lockState = CursorLockMode.Locked; // при желании убери
    }

    void LateUpdate()
    {
        if(Locks()) return; // Если закрыто, то не крутим камерой

		// Только горизонтальный поворот мышью
		float mx = Input.GetAxis("Mouse X") * sensitivity;
        yaw += mx;

        // Вращаем позицию вокруг игрока по оси Y, сохраняя дистанцию
        Quaternion yawRot = Quaternion.AngleAxis(yaw, Vector3.up);
        transform.position = player.GetComponent<Transform>().position + yawRot * offset;

        // Вращаем ориентацию камеры вокруг Y, НО сохраняем твой исходный наклон
        transform.rotation = yawRot * initRot;
    }

    bool Locks() // проверка всех условий при которых камера не должна крутиться
	{
		if (GameManager.instance.isPaused) return true;  //если игра на паузе то не двигаем камерой
		if (player.isCasting) return true;  //если кастует спел то камера не работает
		if (player == null) return true;

        return false;
	}
}