using UnityEngine;

/// <summary>
/// A Base do Tonho (safe zone). Tem vida; quando um zumbi encosta nela ela toma dano.
/// Se a vida zerar, é Game Over.
/// Requer: Collider2D com "Is Trigger" marcado.
/// </summary>
public class BaseTonho : MonoBehaviour
{
    public static BaseTonho Instance { get; private set; }

    public int maxLife = 10;
    public int Life { get; private set; }

    void Awake()
    {
        Instance = this;
        Life = maxLife;
    }

    /// <summary>Detecção de colisão: um zumbi entrou na área da base.</summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
            enemy.ReachBase();
    }

    public void TakeDamage(int amount)
    {
        Life = Mathf.Max(0, Life - amount);
        if (Life == 0)
            GameManager.Instance.Lose("A Base do Tonho foi destruída!");
    }
}
