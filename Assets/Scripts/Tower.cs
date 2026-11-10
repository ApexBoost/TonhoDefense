using UnityEngine;

/// <summary>Estratégia de seleção de alvo da torre.</summary>
public enum TargetMode
{
    First,      // o zumbi mais avançado no caminho (mais perto da base)
    Strongest,  // o zumbi com mais vida
    Closest     // o zumbi mais perto da torre
}

/// <summary>
/// Torre de defesa. Detecta zumbis no raio, escolhe um alvo, mira e atira.
/// O mesmo script faz a Torre Básica e o Canhão: só mudam os valores no Inspector.
/// </summary>
public class Tower : MonoBehaviour
{
    [Header("Informações")]
    public string towerName = "Torre";
    public int cost = 50;

    [Header("Ataque")]
    public float range = 3f;            // raio de detecção
    public int damage = 1;              // dano por tiro
    public float fireRate = 2f;         // tiros por segundo
    public TargetMode targetMode = TargetMode.First;

    [Header("Projétil")]
    public Projectile projectilePrefab;
    public Transform firePoint;         // de onde o tiro sai (opcional)

    float cooldown;                     // tempo até o próximo tiro

    void Update()
    {
        cooldown -= Time.deltaTime;

        Enemy target = FindTarget();
        if (target == null) return;

        AimAt(target);

        if (cooldown <= 0f)
        {
            Shoot(target);
            cooldown = 1f / fireRate;
        }
    }

    /// <summary>Detecta os zumbis dentro do raio e escolhe um conforme a estratégia.</summary>
    Enemy FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        Enemy best = null;
        float bestScore = float.NegativeInfinity;

        foreach (Collider2D col in hits)
        {
            if (!col.TryGetComponent(out Enemy enemy)) continue;

            float score = 0f;
            switch (targetMode)
            {
                case TargetMode.First:
                    score = enemy.PathProgress;
                    break;
                case TargetMode.Strongest:
                    score = enemy.CurrentHealth * 100000f + enemy.PathProgress;
                    break;
                case TargetMode.Closest:
                    score = -Vector2.Distance(transform.position, enemy.transform.position);
                    break;
            }

            if (score > bestScore)
            {
                bestScore = score;
                best = enemy;
            }
        }
        return best;
    }

    /// <summary>Gira a torre para o alvo (o "cano" aponta para o eixo X local).</summary>
    void AimAt(Enemy target)
    {
        Vector2 dir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Shoot(Enemy target)
    {
        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        Projectile p = Instantiate(projectilePrefab, origin, Quaternion.identity);
        p.Init(target, damage);
    }

    // Mostra o alcance na aba Scene quando a torre está selecionada
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
