using UnityEngine;

/// <summary>
/// Espaço pré-definido onde o jogador pode construir uma torre.
/// - Ponto central: a posição deste objeto.
/// - Área de detecção: o Collider2D (CircleCollider2D) deste objeto.
/// - Disponibilidade: IsOccupied.
/// - Referência da torre construída: BuiltTower.
/// </summary>
public class TowerSlot : MonoBehaviour
{
    public Tower BuiltTower { get; private set; }

    // Livre quando não há torre (se a torre for destruída/vendida, o slot volta a ficar livre)
    public bool IsOccupied => BuiltTower != null;

    SpriteRenderer marker;      // marcador azul que indica "pode construir aqui"
    Color freeColor;

    void Awake()
    {
        marker = GetComponentInChildren<SpriteRenderer>();
        if (marker != null) freeColor = marker.color;
    }

    /// <summary>Registra a torre construída e apaga o marcador.</summary>
    public void Occupy(Tower tower)
    {
        BuiltTower = tower;
        if (marker != null)
        {
            Color c = freeColor;
            c.a = 0.05f;
            marker.color = c;
        }
    }

    /// <summary>Libera o espaço (uso futuro: vender/remover torre).</summary>
    public void Free()
    {
        BuiltTower = null;
        if (marker != null) marker.color = freeColor;
    }
}
