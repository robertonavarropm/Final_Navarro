using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Timing")]
    public float fuseTime = 2f;        // tiempo antes de explotar
    public float cellSize = 1f;        // tamaño de celda (tu grilla)
    public int range = 2;              // alcance en celdas

    [Header("Explosion blocking")]
    public LayerMask wallMask;         // paredes/bloques indestructibles

    void Start()
    {
        Invoke(nameof(Explode), fuseTime);
    }

    void Explode()
    {
        // Centro
        DamageAt(transform.position);

        // Cruz (vectores cardinales)
        ExplodeDir(Vector3.forward);
        ExplodeDir(Vector3.back);
        ExplodeDir(Vector3.left);
        ExplodeDir(Vector3.right);

        Destroy(gameObject);
    }

    void ExplodeDir(Vector3 dir)
    {
        for (int i = 1; i <= range; i++)
        {
            Vector3 pos = transform.position + dir * (i * cellSize);

            // Si hay pared, se corta la explosión
            if (Physics.CheckBox(pos, Vector3.one * 0.45f, Quaternion.identity, wallMask))
                break;

            DamageAt(pos);
        }
    }

    void DamageAt(Vector3 pos)
    {
        Collider[] hits = Physics.OverlapBox(pos, Vector3.one * 0.45f);

        foreach (var h in hits)
        {
            // Matar enemigos
            var enemy = h.GetComponentInParent<EnemyController>();
            if (enemy != null)
                enemy.TakeDamage(1);

            // (Opcional) matar jugador
            if (h.CompareTag("Player"))
                Destroy(h.gameObject);
        }
    }
}