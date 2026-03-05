using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    public float fuseTime = 2f;
    public int range = 2;
    public float cellSize = 1f;

    [Header("Layers")]
    public LayerMask wallMask;
    public LayerMask destructibleMask;

    void Start()
    {
        Invoke(nameof(Explode), fuseTime);
    }

    void Explode()
    {
        // centro de la explosión
        DamageAndBreak(transform.position + Vector3.up * 0.5f);

        // explosión en cruz usando vectores
        ExplodeDirection(Vector3.forward);
        ExplodeDirection(Vector3.back);
        ExplodeDirection(Vector3.left);
        ExplodeDirection(Vector3.right);

        Destroy(gameObject);
    }

    void ExplodeDirection(Vector3 dir)
    {
        for (int i = 1; i <= range; i++)
        {
            Vector3 pos = transform.position + dir * (i * cellSize);
            Vector3 checkPos = pos + Vector3.up * 0.5f;

            // si hay pared indestructible se corta
            if (Physics.CheckBox(checkPos, Vector3.one * 0.45f, Quaternion.identity, wallMask))
                break;

            bool brokeBlock = DamageAndBreak(checkPos);

            // si rompió bloque destructible se corta
            if (brokeBlock)
                break;
        }
    }

    bool DamageAndBreak(Vector3 pos)
    {
        bool broke = false;

        Collider[] hits = Physics.OverlapBox(pos, Vector3.one * 0.45f);

        foreach (var h in hits)
        {
            // matar enemigo
            EnemyController enemy = h.GetComponentInParent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }

            // matar jugador
            PlayerHealth player = h.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.Die();
            }

            // romper bloque destructible
            if (((1 << h.gameObject.layer) & destructibleMask) != 0)
            {
                Destroy(h.gameObject);
                broke = true;
            }
        }

        return broke;
    }
}