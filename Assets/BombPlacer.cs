using UnityEngine;
using UnityEngine.InputSystem;

public class BombPlacer : MonoBehaviour
{
    public GameObject bombPrefab;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Vector3 pos = transform.position;
            pos.y = 0;

            Instantiate(bombPrefab, pos, Quaternion.identity);
        }
    }
}