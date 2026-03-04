using UnityEngine;

public class PlayerDieOnEnemyTouch : MonoBehaviour
{
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Enemy"))
            Destroy(gameObject);
    }
}