using UnityEngine;

/// <summary>
/// Zumbi. Anda pelos waypoints do caminho até a base, tem vida e some quando morre.
/// O mesmo script serve para o zumbi normal e para o tanque: só mudam os valores no Inspector.
/// Requer no prefab: SpriteRenderer, Collider2D (NÃO trigger) e Rigidbody2D (Kinematic).
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Atributos (mude por prefab)")]
    public float speed = 2f;        // velocidade de movimento
    public int maxHealth = 3;       // vida máxima
    public int damageToBase = 1;    // dano causado na base ao chegar
    public int reward = 10;         // dinheiro ganho ao ser morto

    public int CurrentHealth { get; private set; }

    EnemyPath path;
    int waypointIndex = 1;          // índice 0 é o ponto de nascimento; o primeiro destino é o 1
    bool finished;                  // já morreu ou já chegou na base
    SpriteRenderer sr;
    Color baseColor;

    /// <summary>
    /// Quanto o zumbi avançou no caminho (maior = mais perto da base).
    /// A torre usa isso para atirar no zumbi que está mais à frente.
    /// </summary>
    public float PathProgress
    {
        get
        {
            if (finished || path == null) return 0f;
            return waypointIndex * 1000f -
                   Vector2.Distance(transform.position, path.waypoints[waypointIndex].position);
        }
    }

    void Awake()
    {
        CurrentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) baseColor = sr.color;
    }

    /// <summary>Chamado pelo WaveSpawner logo depois de criar o zumbi.</summary>
    public void Init(EnemyPath enemyPath)
    {
        path = enemyPath;
        transform.position = path.waypoints[0].position;
        waypointIndex = 1;
    }

    void Update()
    {
        if (path == null || finished) return;

        // Anda em direção ao waypoint atual
        Vector3 targetPos = path.waypoints[waypointIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Chegou no waypoint? Passa para o próximo
        if (Vector3.Distance(transform.position, targetPos) < 0.02f)
        {
            waypointIndex++;
            // Acabou o caminho: chegou na base (garantia caso o trigger da base falhe)
            if (waypointIndex >= path.waypoints.Length) ReachBase();
        }
    }

    /// <summary>Recebe dano (chamado pelo projétil). Morre se a vida zerar.</summary>
    public void TakeDamage(int amount)
    {
        if (finished) return;

        CurrentHealth -= amount;

        // Feedback visual: escurece conforme perde vida
        if (sr != null)
        {
            float ratio = Mathf.Clamp01((float)CurrentHealth / maxHealth);
            sr.color = Color.Lerp(new Color(0.15f, 0.15f, 0.15f), baseColor, ratio);
        }

        if (CurrentHealth <= 0) Die();
    }

    void Die()
    {
        finished = true;
        GameManager.Instance.EnemyKilled(reward);
        Destroy(gameObject);
    }

    /// <summary>Chamado pela Base (trigger) ou pelo fim do caminho: causa dano e remove o zumbi.</summary>
    public void ReachBase()
    {
        if (finished) return;
        finished = true;
        if (BaseTonho.Instance != null) BaseTonho.Instance.TakeDamage(damageToBase);
        GameManager.Instance.EnemyReachedBase();
        Destroy(gameObject);
    }
}
