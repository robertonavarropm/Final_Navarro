using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerTouchEnemyKill : MonoBehaviour
{
    private PlayerHealth health;

    void Awake()
    {
        health = GetComponent<PlayerHealth>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Enemy"))
        {
            health.Die();
        }
    }
}