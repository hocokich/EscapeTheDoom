using UnityEngine;

public class MachinegunLogic : MonoBehaviour
{
	[SerializeField] LayerMask enemy;    //слой, на котором расположены противники
	[Range(1, 20)]
	public float piercingPower;    //пробивная сила пуль (сколько противников может пробить пуля)

	public void shot(Transform firePoint, float damage)
	{
		RaycastHit[] hits;

		Ray ray = new Ray(firePoint.position, firePoint.forward);    //луч, проходящий из точки выстрела по направлению выстрела
		hits = Physics.RaycastAll(ray, 100f, enemy);    //обнаружение всех пересечений луча и противников

		System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));    //сортировка пересечений по удалённости от игрока

		if (hits.Length > 0)
		{
			for (int i = 0; i < Mathf.Min(piercingPower, hits.Length); i++)    //для каждого пересечения, начиная с ближайшего
			{
				Health enemy = hits[i].transform.GetComponent<Health>();

				enemy.hpDecrease(damage);
				
				//Destroy(hits[i].transform); // убиваем врага

				/*				Health enemyHP = hits[i].transform.GetComponent<Health>();    //попытка получить ссылку на здоровье противника
								if (enemyHP != null)
								{
									enemyHP.hpDecrease(damage);    //уменьшение здоровья противника
								}*/
			}
		}
	}
}