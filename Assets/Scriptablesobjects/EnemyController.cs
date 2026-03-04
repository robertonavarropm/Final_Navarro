using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("Config")]
    public EnemyData data;
    public LayerMask wallMask;
    public Transform player; // arrastrar el Player acá (o se auto-busca por tag)

    [Header("Chase")]
    public float sightDistance = 20f;      // hasta dónde “ve”
    public float repathInterval = 0.25f;   // cada cuánto reconsidera dirección

    private Rigidbody rb;
    private int hp;

    private Vector3 dir;
    private float nextThinkTime;

    private static readonly Vector3[] dirs =
    {
        Vector3.forward, Vector3.back, Vector3.left, Vector3.right
    };

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (data == null)
        {
            UnityEngine.Debug.LogError("EnemyController: falta asignar EnemyData.", this);
            enabled = false;
            return;
        }

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        hp = data.maxHp;
        PickRandomDirection();
        nextThinkTime = Time.time;
    }

    void FixedUpdate()
    {
        // 1) Pensar: elegir dirección (persigue si puede “ver” al jugador)
        if (Time.time >= nextThinkTime)
        {
            Vector3 desired = GetChaseDirectionOrZero();
            if (desired != Vector3.zero)
                dir = desired;
            else if (IsBlocked(dir))
                PickRandomDirection();

            nextThinkTime = Time.time + repathInterval;
        }

        // 2) Si justo se bloqueó, cambia
        if (IsBlocked(dir))
            PickRandomDirection();

        // 3) Movimiento vectorial
        Vector3 delta = dir.normalized * data.moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + delta);
    }

    // Devuelve la dirección cardinal para perseguir si tiene línea de visión, si no Vector3.zero
    Vector3 GetChaseDirectionOrZero()
    {
        if (player == null) return Vector3.zero;

        Vector3 toPlayer = player.position - transform.position;

        // Proyectamos al plano XZ (bomberman top-down)
        toPlayer.y = 0f;

        // Si está muy cerca, no hace falta “ver”
        if (toPlayer.sqrMagnitude < 0.01f) return Vector3.zero;

        // Elegimos eje dominante para decidir si estamos “en la misma fila/columna”
        float ax = Mathf.Abs(toPlayer.x);
        float az = Mathf.Abs(toPlayer.z);

        // Umbral para considerar "misma fila/columna" (ajustá según tamaño de celda)
        float sameLineThreshold = 0.35f;

        Vector3 chaseDir = Vector3.zero;

        if (ax < sameLineThreshold)
        {
            chaseDir = (toPlayer.z > 0f) ? Vector3.forward : Vector3.back;
        }
        else if (az < sameLineThreshold)
        {
            chaseDir = (toPlayer.x > 0f) ? Vector3.right : Vector3.left;
        }
        else
        {
            // Si no está alineado, no perseguimos “perfecto bomberman”
            return Vector3.zero;
        }

        // Línea de visión: rayo hacia el jugador. Si choca pared antes, no lo ve.
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        float dist = Mathf.Min(sightDistance, toPlayer.magnitude);

        if (Physics.Raycast(origin, chaseDir, out RaycastHit hit, dist, wallMask))
        {
            // Hay pared en medio
            return Vector3.zero;
        }

        // Además evitamos elegir una dirección bloqueada inmediata
        if (IsBlocked(chaseDir)) return Vector3.zero;

        return chaseDir;
    }

    bool IsBlocked(Vector3 d)
    {
        if (d == Vector3.zero) return true;

        Vector3 origin = transform.position + Vector3.up * 0.2f;
        return Physics.Raycast(origin, d, data.obstacleCheckDistance, wallMask);
    }

    void PickRandomDirection()
    {
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

    // Vida
    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0) Destroy(gameObject);
    }

    // Tocar jugador = matar jugador
    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Player"))
            Destroy(col.collider.gameObject);
    }
}