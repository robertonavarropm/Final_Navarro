using UnityEngine;

public class Bomb : MonoBehaviour
{
    public BombData data;

    [Header("Collision masks")]
    public LayerMask wallMask;          // bloquea explosión
    public LayerMask destructibleMask;  // opcional: rompe bloques

    [Header("Damage")]
    public int enemyDamage = 1;
    public bool killPlayer = true;

    void Start()
    {
        Invoke(nameof(Explode), data.fuseTime);
    }

    void Explode()
    {
        // Centro (daño en la celda de la bomba)
        ApplyDamageAt(transform.position);

        // Explosión en cruz (vectores cardinales)
        ExplodeDirection(Vector3.forward);
        ExplodeDirection(Vector3.back);
        ExplodeDirection(Vector3.left);
        ExplodeDirection(Vector3.right);

        Destroy(gameObject);
    }

    void ExplodeDirection(Vector3 dir)
    {
        // dir es un vector unitario cardinal
        for (int i = 1; i <= data.range; i++)
        {
            Vector3 pos = transform.position + dir * (i * data.cellSize);

            // Si hay pared, se corta la explosión
            if (Physics.CheckBox(pos, Vector3.one * 0.45f, Quaternion.identity, wallMask))
                break;

            // Daña lo que esté en esa celda
            ApplyDamageAt(pos);

            // Si hay bloque destructible, lo rompe y corta (estilo bomberman)
            Collider[] destructibles = Physics.OverlapBox(pos, Vector3.one * 0.45f, Quaternion.identity, destructibleMask);
            if (destructibles.Length > 0)
            {
                foreach (var d in destructibles)
                    Destroy(d.gameObject);

                break;
            }
        }
    }

    void ApplyDamageAt(Vector3 pos)
    {
        // Enemigos
        Collider[] hits = Physics.OverlapBox(pos, Vector3.one * 0.45f, Quaternion.identity);
        foreach (var h in hits)
        {
            var enemy = h.GetComponentInParent<EnemyController>();
            if (enemy != null)
                enemy.TakeDamage(enemyDamage);

            if (killPlayer && h.CompareTag("Player"))
                Destroy(h.gameObject);
        }
    }

    // Para ver el “hit” en escena (opcional)
    void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.9f);
    }
}