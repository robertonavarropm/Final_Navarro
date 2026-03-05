using UnityEngine;
using UnityEngine.InputSystem;

public class BombPlacer : MonoBehaviour
{
    public GameObject bombPrefab;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (bombPrefab == null)
            {
                Debug.LogError("BombPlacer: NO hay bombPrefab asignado (arrastrá el Bomb.prefab desde Assets).", this);
                return;
            }

            Vector3 pos = transform.position;
            pos.y = 0f;

            Instantiate(bombPrefab, pos, Quaternion.identity);
        }
    }
}