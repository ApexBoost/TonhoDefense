using UnityEngine;

/// <summary>
/// Espaço pré-definido onde o jogador pode construir uma torre.
/// Requer: SpriteRenderer e Collider2D (NÃO trigger).
/// </summary>
public class TowerSlot : MonoBehaviour
{
    public bool IsOccupied { get; private set; }

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>Marca o espaço como ocupado e deixa o quadrado mais apagado.</summary>
    public void Occupy()
    {
        IsOccupied = true;
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0.25f;
            sr.color = c;
        }
    }
}
