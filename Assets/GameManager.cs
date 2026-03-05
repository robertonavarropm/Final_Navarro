using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint;

    private GameObject currentPlayer;

    void Start()
    {
        // Si ya hay un jugador en la escena, usalo y no spawnees otro
        GameObject existing = GameObject.FindGameObjectWithTag("Player");
        if (existing != null)
        {
            currentPlayer = existing;
        }
        else
        {
            SpawnPlayer();
        }
    }

    void Update()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            RespawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
    }

    void RespawnPlayer()
    {
        if (currentPlayer != null)
            Destroy(currentPlayer);

        SpawnPlayer();
    }
}