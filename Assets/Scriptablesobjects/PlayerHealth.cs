using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public void Die()
    {
        Debug.Log("Player murió");
        Destroy(gameObject);
    }
}