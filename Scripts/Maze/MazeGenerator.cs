using UnityEngine;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class MazeGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float roomSize = 4f; // ? добавил недостающий параметр
    public int Keys = 0;        // ключи для прохождения уровня

    [Header("Prefabs")]
	public GameObject[] roomPrefabs;
    public GameObject[] enemyPrefabs;
    public GameObject exitPrefab;
    public GameObject KeyPrefab;
    public int enemyCount = 5; // сколько врагов появится

    [Header("NavMesh")]
    public NavMeshSurface surface;

    private Room[,] grid;
    private bool[,] visited;

    void Start()
    {
        // Берём параметры из GameManager (если он есть)
        if (GameManager.instance != null)
        {
            width = GameManager.instance.mazeSize;
            height = GameManager.instance.mazeSize;

            // Сложность врагов и кол-во ключей растёт с уровнем
            enemyCount = Mathf.Max(1, GameManager.instance.level * 3);
            Keys = GameManager.instance.level * 1;
		}

        if(GameManager.instance.level > 2)
            GenerateMaze(1);
        else GenerateMaze(0);

		if (surface != null)
			surface.BuildNavMesh();

		SpawnEnemies(2);

        SpawnKeys();
	}

    void GenerateMaze(int index)
    {
        grid = new Room[width, height];
        visited = new bool[width, height];

        // 1. Создаём сетку комнат
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * roomSize, 0, y * roomSize);
                GameObject obj = Instantiate(roomPrefabs[index], pos, Quaternion.identity, transform);
                Room room = obj.GetComponent<Room>();

                // --- Убираем "двойные стены" ---
                if (x > 0) room.wallWest.SetActive(false);  // уже есть стена у соседа слева
                if (y > 0) room.wallSouth.SetActive(false); // уже есть стена у соседа снизу

                grid[x, y] = room;
            }
        }

        // 2. Алгоритм DFS (Depth-First Search)
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int current = new Vector2Int(0, 0);
        visited[0, 0] = true;
        stack.Push(current);

        while (stack.Count > 0)
        {
            current = stack.Pop();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                stack.Push(current);

                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                RemoveWall(current, next);
                visited[next.x, next.y] = true;
                stack.Push(next);
            }
        }

        // Выход в противоположном углу
        Vector3 exitPos = new Vector3((width - 1) * roomSize, 0, (height - 1) * roomSize + 2);
        Instantiate(exitPrefab, exitPos, Quaternion.identity);
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (cell.x > 0 && !visited[cell.x - 1, cell.y]) neighbors.Add(new Vector2Int(cell.x - 1, cell.y));
        if (cell.x < width - 1 && !visited[cell.x + 1, cell.y]) neighbors.Add(new Vector2Int(cell.x + 1, cell.y));
        if (cell.y > 0 && !visited[cell.x, cell.y - 1]) neighbors.Add(new Vector2Int(cell.x, cell.y - 1));
        if (cell.y < height - 1 && !visited[cell.x, cell.y + 1]) neighbors.Add(new Vector2Int(cell.x, cell.y + 1));

        return neighbors;
    }

    void RemoveWall(Vector2Int a, Vector2Int b)
    {
        if (a.x == b.x) // по вертикали
        {
            if (a.y < b.y)
            {
                grid[a.x, a.y].wallNorth.SetActive(false);
                grid[b.x, b.y].wallSouth.SetActive(false);
            }
            else
            {
                grid[a.x, a.y].wallSouth.SetActive(false);
                grid[b.x, b.y].wallNorth.SetActive(false);
            }
        }
        else if (a.y == b.y) // по горизонтали
        {
            if (a.x < b.x)
            {
                grid[a.x, a.y].wallEast.SetActive(false);
                grid[b.x, b.y].wallWest.SetActive(false);
            }
            else
            {
                grid[a.x, a.y].wallWest.SetActive(false);
                grid[b.x, b.y].wallEast.SetActive(false);
            }
        }
    }

    void SpawnEnemies(int index)
    {
        if (enemyPrefabs == null) return;

        for (int i = 0; i < enemyCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            // избегаем старта (0,0) и выхода (width-1,height-1)
            if ((x == 0 && y == 0) || (x == width - 1 && y == height - 1))
                continue;
            
            Vector3 pos = new Vector3(x * roomSize, 0f, y * roomSize);
            Instantiate(enemyPrefabs[index], pos, Quaternion.identity, transform);
        }
    }

	void SpawnKeys()
	{
		if (KeyPrefab == null || Keys <= 0) return;

		List<Vector2Int> usedPositions = new List<Vector2Int>();

		for (int i = 0; i < Keys; i++)
		{
			int x, y;
			int attempts = 0;
			bool validPosition = false;
			Vector2Int newPos;

			do
			{
				x = Random.Range(0, width);
				y = Random.Range(0, height);
				newPos = new Vector2Int(x, y);

				// Проверяем что не на старте, не на выходе и не на занятой позиции
				validPosition = !((x == 0 && y == 0) ||
								(x == width - 1 && y == height - 1) ||
								usedPositions.Contains(newPos));
				attempts++;
			}
			while (!validPosition && attempts < 100);

			if (validPosition)
			{
				Vector3 pos = new Vector3(x * roomSize, 0f, y * roomSize);
				Instantiate(KeyPrefab, pos, Quaternion.identity, transform);
				usedPositions.Add(newPos);
			}
		}
	}
}