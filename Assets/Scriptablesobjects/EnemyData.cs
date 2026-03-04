using UnityEngine;

[CreateAssetMenu(menuName = "Bomberman/Enemy Data", fileName = "EnemyData_")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public int maxHp = 1;
    public float moveSpeed = 3f;
    public int contactDamage = 1;

    [Header("Movement (Bomberman-like)")]
    public float obstacleCheckDistance = 0.6f;
    public float directionChangeCooldown = 0.2f;
}