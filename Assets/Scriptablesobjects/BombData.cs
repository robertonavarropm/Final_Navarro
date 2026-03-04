using UnityEngine;

[CreateAssetMenu(menuName = "Bomberman/Bomb Data", fileName = "BombData_")]
public class BombData : ScriptableObject
{
    [Header("Timing")]
    public float fuseTime = 2.0f;      // tiempo hasta explotar
    public float explosionTime = 0.2f; // duración visual/daño (si usás hitboxes)

    [Header("Explosion")]
    public int range = 2;              // alcance en celdas
    public float cellSize = 1f;        // tamaño de celda/grilla
}