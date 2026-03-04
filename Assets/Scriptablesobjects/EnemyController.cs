using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("Config")]
    public EnemyData data;       // ScriptableObject con stats
    public LayerMask wallMask;   // Layer de paredes/obstáculos

    private Rigidbody rb;
    private int hp;

    private Vector3 dir;
    private float nextDirChangeTime;

    // Direcciones tipo Bomberman (4 cardinales)
    private static readonly Vector3[] dirs =
    {
        Vector3.forward, // (0,0,1)
        Vector3.back,    // (0,0,-1)
        Vector3.left,    // (-1,0,0)
        Vector3.right    // (1,0,0)
    };

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Recomendado para top-down
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (data == null)
        {
            UnityEngine.Debug.LogError("EnemyController: falta asignar EnemyData en el Inspector.", this);
            enabled = false;
            return;
        }

        hp = data.maxHp;
        PickNewDirection();
    }

    void FixedUpdate()
    {
        // Si hay pared adelante, cambiar de dirección (estilo Bomberman)
        if (Time.time >= nextDirChangeTime && IsWallAhead())
        {
            PickNewDirection();
            nextDirChangeTime = Time.time + data.directionChangeCooldown;
        }

        // Matemática de vectores: delta = dirección * velocidad * dt
        Vector3 moveDir = dir.normalized; // unitario
        Vector3 delta = moveDir * data.moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + delta);
    }

    private bool IsWallAhead()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        return Physics.Raycast(origin, dir, data.obstacleCheckDistance, wallMask);
    }

    private void PickNewDirection()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;

        // Intentar encontrar una dirección libre
        for (int i = 0; i < 10; i++)
        {
            Vector3 candidate = dirs[Random.Range(0, dirs.Length)];

            if (!Physics.Raycast(origin, candidate, data.obstacleCheckDistance, wallMask))
            {
                dir = candidate;
                return;
            }
        }

        // Fallback si está “encerrado”
        dir = dirs[Random.Range(0, dirs.Length)];
    }

    // --- VIDA ENEMIGO ---
    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("Choqué con: " + col.collider.name);

        if (col.collider.CompareTag("Player"))
        {
            Debug.Log("¡Toqué al Player!");

            PlayerHealth player = col.collider.GetComponent<PlayerHealth>();
            if (player != null) player.Die();
            else Destroy(col.collider.gameObject);
        }
    }
}