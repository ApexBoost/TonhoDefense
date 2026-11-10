using UnityEngine;

/// <summary>
/// Projétil: criado pela torre, vai até o zumbi (translação), detecta a colisão,
/// causa dano e se destrói.
/// Requer no prefab: Collider2D com "Is Trigger" marcado e Rigidbody2D (Kinematic).
/// </summary>
public class Projectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifetime = 5f;   // segurança: some sozinho se algo der errado

    Enemy target;
    int damage;
    bool hit;

    /// <summary>Chamado pela torre ao disparar.</summary>
    public void Init(Enemy newTarget, int newDamage)
    {
        target = newTarget;
        damage = newDamage;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // O alvo morreu antes do projétil chegar? Remove o projétil.
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move em direção ao alvo (projétil teleguiado)
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    /// <summary>Detecção de colisão com o inimigo.</summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (hit) return;
        if (other.TryGetComponent(out Enemy enemy))
        {
            hit = true;
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
