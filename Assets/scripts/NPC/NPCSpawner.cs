using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private int npcCount = 5;

    [Header("Spawn Area")]
    [SerializeField] private float spawnRadius = 20f;

    private void Start()
    {
        SpawnNPCs();
    }

    private void SpawnNPCs()
    {
        for (int i = 0; i < npcCount; i++)
        {
            Vector3 randomPosition =
                transform.position +
                Random.insideUnitSphere * spawnRadius;

            randomPosition.y = transform.position.y;

            if (NavMesh.SamplePosition(
                randomPosition,
                out NavMeshHit hit,
                spawnRadius,
                NavMesh.AllAreas))
            {
                Instantiate(
                    npcPrefab,
                    hit.position,
                    Quaternion.identity
                );
            }
        }
    }
}