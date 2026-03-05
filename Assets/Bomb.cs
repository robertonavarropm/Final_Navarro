using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Timing")]
    public float fuseTime = 2f;
    public float cellSize = 1f;
    public int range = 2;

    [Header("Explosion blocking / breaking")]
    public LayerMask wallMask;          // paredes indestructibles (cortan)
    public LayerMask destructibleMask;  // bloques destructibles (rompen y cortan)

    [Header("Damage")]
    public int enemyDamage = 1;
    public bool killPlayer = true;

    void Start()
    {
        Invoke(nameof(Explode), fuseTime);
    }

    void Explode()
    {
        // Centro
        DamageAndBreakAt(transform.position);

        // Cruz (vectores)
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

            // 1) Pared indestructible: corta
            if (Physics.CheckBox(pos, Vector3.one * 0.45f, Quaternion.identity, wallMask))
                break;

            // 2) Aplicar daño + romper si hay destructible
            bool brokeSomething = DamageAndBreakAt(pos);

            // 3) Si rompió un bloque destructible, la explosión se corta (Bomberman)
            if (brokeSomething)
                break;
        }
    }

    // Devuelve true si rompió al menos 1 destructible en esa celda
    bool DamageAndBreakAt(Vector3 pos)
    {
        bool broke = false;

        // Enemigos / jugador (daño)
        Collider[] hits = Physics.OverlapBox(pos, Vector3.one * 0.45f);
        foreach (var h in hits)
        {
            var enemy = h.GetComponentInParent<EnemyController>();
            if (enemy != null)
                enemy.TakeDamage(enemyDamage);

            if (killPlayer && h.CompareTag("Player"))
                Destroy(h.gameObject);
        }

        // Bloques destructibles (romper)
        Collider[] destructibles = Physics.OverlapBox(pos, Vector3.one * 0.45f, Quaternion.identity, destructibleMask);
        foreach (var d in destructibles)
        {
            Destroy(d.gameObject);
            broke = true;
        }

        return broke;
    }
}