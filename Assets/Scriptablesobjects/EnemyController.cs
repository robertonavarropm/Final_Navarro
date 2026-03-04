using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("Config")]
    public EnemyData data;
    public LayerMask wallMask;

    Rigidbody rb;
    int hp;

    Vector3 dir;
    float nextDirChangeTime;

    static readonly Vector3[] dirs =
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right
    };

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Seguridad por si te olvidás de asignar data
        if (data == null)
        {
            Debug.LogError("EnemyController: falta asignar EnemyData.", this);
            enabled = false;
            return;
        }

        hp = data.maxHp;
        PickNewDirection();
    }

    void FixedUpdate()
    {
        // Si hay pared adelante, cambiá dirección (estilo Bomberman)
        if (Time.time >= nextDirChangeTime && IsWallAhead())
        {
            PickNewDirection();
            nextDirChangeTime = Time.time + data.directionChangeCooldown;
        }

        // Matemática vectorial: delta = dir * speed * dt
        Vector3 moveDir = dir.normalized; // (unitario)
        Vector3 delta = moveDir * data.moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + delta);
    }

    bool IsWallAhead()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        return Physics.Raycast(origin, dir, data.obstacleCheckDistance, wallMask);
    }

    void PickNewDirection()
    {
        // probamos varias para evitar quedarnos eligiendo una dirección bloqueada
        Vector3 origin = transform.position + Vector3.up * 0.2f;

        for (int i = 0; i < 10; i++)
        {
            Vector3 candidate = dirs[Random.Range(0, dirs.Length)];
            if (!Physics.Raycast(origin, candidate, data.obstacleCheckDistance, wallMask))
            {
                dir = candidate;
                return;
            }
        }

        dir = dirs[Random.Range(0, dirs.Length)];
    }

    // --- VIDA ---
    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0) Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    // --- ATAQUE AL CONTACTO ---
    void OnCollisionEnter(Collision col)
    {
        if (!col.collider.CompareTag("Player")) return;

        // si tu jugador tiene vida:
        var ph = col.collider.GetComponent<PlayerHealth>();
        if (ph != null) ph.TakeDamage(data.contactDamage);
        else
        {
            // si no tiene vida, lo destruimos (bomberman clásico: te mata al toque)
            Destroy(col.collider.gameObject);
        }
    }
}