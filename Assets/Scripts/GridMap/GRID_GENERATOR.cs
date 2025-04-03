using System.Collections.Generic;
using UnityEngine;

public class GRID_GENERATOR : MonoBehaviour
{
    public GameObject[] roadPrefabs; // Prefabs del camino
    public GameObject[] buildingPrefabs; // Prefabs de edificios
    public int gridWidth = 7; // Ancho total de la cuadrícula (debe ser impar para centrar mejor)
    public int gridHeight = 5; // Número de filas visibles al inicio
    public float cellSize = 5f; // Tamaño de cada celda
    public Transform player; // Referencia al jugador

    private Dictionary<Vector2, GameObject> activeTiles = new Dictionary<Vector2, GameObject>(); // Tiles activos
    private float spawnZ = 0f; // Posición Z de la siguiente fila a generar
    private float safeZone = 5f; // Distancia antes de eliminar filas

    private int roadWidth; // Ancho del camino dinámico
    private int roadStartX; // Posición X donde comienza el camino

    public GameObject enemyPrefab; // Prefab del enemigo
    public float enemySpawnChance = 0.2f; // Probabilidad de que un enemigo aparezca en cada fila
    private Dictionary<Vector2, GameObject> activeEnemies = new Dictionary<Vector2, GameObject>(); // Enemigos activos


    void Start()
    {
        // Ajustar el ancho del camino a aproximadamente el 60% del grid
        roadWidth = Mathf.Max(3, gridWidth * 70 / 100); // Mínimo de 3 casillas de ancho
        if (roadWidth % 2 == 0) roadWidth--; // Asegurar que el ancho sea impar para mejor centrado

        // Iniciar el camino en el centro del grid
        roadStartX = (gridWidth - roadWidth) / 2;

        // Posicionar al jugador al inicio del camino
        float playerStartX = (gridWidth / 2) * cellSize; // Centro del grid
        player.position = new Vector3(playerStartX, player.position.y, 0);

        // Generar la cuadrícula inicial
        for (int z = 0; z < gridHeight; z++)
        {
            SpawnRow();
        }
    }

    void Update()
    {
        if (player.position.z - safeZone > (spawnZ - gridHeight * cellSize))
        {
            SpawnRow();
            DeleteOldRow();
        }
    }

    void SpawnRow()
    {
        // Desviación aleatoria del camino con limitaciones para evitar salirse del grid
        int direction = Random.Range(0, 3); // 0 = izquierda, 1 = recto, 2 = derecha

        if (direction == 0 && roadStartX > 1) // Mover a la izquierda si hay espacio
        {
            roadStartX--;
        }
        else if (direction == 2 && roadStartX + roadWidth < gridWidth - 1) // Mover a la derecha si hay espacio
        {
            roadStartX++;
        }

        for (int x = 0; x < gridWidth; x++)
        {
            float spawnX = (x - (gridWidth / 2)) * cellSize; // Centrar en X
            Vector2 key = new Vector2(spawnX, spawnZ);

            if (!activeTiles.ContainsKey(key))
            {
                GameObject prefab;

                // Si la casilla está dentro del camino, colocar suelo de carretera
                if (x >= roadStartX && x < roadStartX + roadWidth)
                {
                    prefab = roadPrefabs[Random.Range(0, roadPrefabs.Length)];

                    // Generar un enemigo dentro del camino con una probabilidad
                    if (Random.value < enemySpawnChance)
                    {
                        // Generar el enemigo sobre el camino (ajustando Y a un valor mayor)
                        Vector3 enemyPosition = new Vector3(spawnX, 1f, spawnZ); // Ajusta el valor de Y para la altura deseada

                        // Instanciar al enemigo y agregarlo al diccionario
                        GameObject newEnemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
                        activeEnemies.Add(key, newEnemy);
                    }
                }
                else
                {
                    prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
                }

                GameObject newTile = Instantiate(prefab, new Vector3(spawnX, 0, spawnZ), Quaternion.identity);
                activeTiles.Add(key, newTile);
            }
        }

        spawnZ += cellSize;

        // Eliminar enemigos que ya no están dentro de la zona visible del jugador
        DeleteOldEnemies();
    }

    void DeleteOldEnemies()
    {
        // Recorrer los enemigos activos y eliminar aquellos que ya están fuera del alcance
        List<Vector2> toRemove = new List<Vector2>();
        foreach (var enemy in activeEnemies)
        {
            // Si el enemigo está más allá de la zona segura
            if (enemy.Key.y <= spawnZ - (gridHeight + 1) * cellSize)
            {
                Destroy(enemy.Value); // Destruir el enemigo
                toRemove.Add(enemy.Key); // Agregar a la lista de eliminación
            }
        }

        // Eliminar los enemigos que han sido destruidos
        foreach (var key in toRemove)
        {
            activeEnemies.Remove(key);
        }
    }

    void DeleteOldRow()
    {
        float deleteZ = spawnZ - (gridHeight + 1) * cellSize;
        List<Vector2> toRemove = new List<Vector2>();

        foreach (var tile in activeTiles)
        {
            if (tile.Key.y <= deleteZ)
            {
                Destroy(tile.Value);
                toRemove.Add(tile.Key);
            }
        }

        foreach (var key in toRemove)
        {
            activeTiles.Remove(key);
        }
    }
}
