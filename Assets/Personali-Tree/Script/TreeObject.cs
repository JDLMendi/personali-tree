using System.Collections.Generic; // Required for List
using UnityEngine;
using Random = UnityEngine.Random;

public class TreeObject : MonoBehaviour
{
    public Vector3[] appleSpawn;
    public int numberOfApples;
    public AppleManager appleManager;
    public TreeManager treeManager;

    // Track which spots are currently taken
    private List<int> _availableIndices = new List<int>();

    public void OnEnable()
    {
        appleManager = GetComponentInChildren<AppleManager>();
        treeManager = FindFirstObjectByType<TreeManager>();
    }

    private void OnDrawGizmosSelected()
    {
        if (appleSpawn == null) return;
        Gizmos.color = Color.red;
        foreach (Vector3 localPos in appleSpawn)
        {
            Vector3 worldPos = transform.TransformPoint(localPos);
            Gizmos.DrawWireSphere(worldPos, 0.5f);
            Gizmos.DrawLine(transform.position, worldPos);
        }
    }

    public void StartTree()
    {
        // 1. Reset the available indices list (Positions)
        _availableIndices.Clear();
        for (int i = 0; i < appleSpawn.Length; i++)
        {
            _availableIndices.Add(i);
        }

        // 2. Create a temporary list of personalities to pick from
        List<SO_Personality> availablePersonalities = new List<SO_Personality>(treeManager.personalities);
    
        // 3. Clamp spawnCount based on BOTH available spots AND available personalities
        // We can't spawn more apples than we have spots OR unique personalities
        int spawnCount = Mathf.Min(numberOfApples, appleSpawn.Length, availablePersonalities.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            // Pick a random personality from our temporary list
            int pIndex = Random.Range(0, availablePersonalities.Count);
            SO_Personality chosenPersonality = availablePersonalities[pIndex];

            // Remove it so it isn't used again for this tree
            availablePersonalities.RemoveAt(pIndex);

            // Spawn the apple with that unique personality
            SpawnApple(chosenPersonality);
        }
    }

    public void SpawnApple(SO_Personality personality)
    {
        if (_availableIndices.Count == 0) return;

        // 3. Pick a random index FROM the list of available spots
        int listIndex = Random.Range(0, _availableIndices.Count);
        int spawnPointIndex = _availableIndices[listIndex];

        // 4. Use the coordinate (Local Space since AppleManager handles placement)
        Vector3 position = appleSpawn[spawnPointIndex];

        // 5. Remove it so it can't be picked again this round
        _availableIndices.RemoveAt(listIndex);

        appleManager.CreateApple(personality, position);
    }
}