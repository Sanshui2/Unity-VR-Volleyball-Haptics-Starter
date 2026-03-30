using System.Collections;
using UnityEngine;

/// <summary>
/// Simple auto-spawning volleyball launcher for the starter scene.
/// </summary>
public class VolleyballSpawner : MonoBehaviour
{
    [Header("Prefab And Interval")]
    [Tooltip("Volleyball prefab to spawn.")]
    public GameObject volleyballPrefab;
    [Tooltip("Seconds between spawns.")]
    public float spawnInterval = 3f;
    [Tooltip("Start spawning automatically when the scene loads.")]
    public bool playOnStart = true;

    [Header("Spawn Settings")]
    [Tooltip("Spawn offset relative to this object.")]
    public Vector3 spawnPositionOffset = Vector3.zero;
    [Tooltip("Launch direction.")]
    public Vector3 spawnDirection = Vector3.down;
    [Tooltip("Initial launch force.")]
    public float initialForce = 5f;

    private Coroutine spawningCoroutine;

    private void Start()
    {
        if (playOnStart)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (spawningCoroutine != null)
        {
            return;
        }

        spawningCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (spawningCoroutine == null)
        {
            return;
        }

        StopCoroutine(spawningCoroutine);
        spawningCoroutine = null;
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnVolleyball();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnVolleyball()
    {
        if (volleyballPrefab == null)
        {
            Debug.LogError("Volleyball prefab is missing.", this);
            return;
        }

        Vector3 actualSpawnPosition = transform.position + spawnPositionOffset;
        GameObject newBall = Instantiate(volleyballPrefab, actualSpawnPosition, Quaternion.identity);
        Rigidbody spawnedBall = newBall.GetComponent<Rigidbody>();
        if (spawnedBall == null)
        {
            return;
        }

        spawnedBall.AddForce(spawnDirection.normalized * initialForce, ForceMode.Impulse);
    }
}
