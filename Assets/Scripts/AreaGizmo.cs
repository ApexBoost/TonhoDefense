using UnityEngine;

/// <summary>
/// Desenha o contorno do Collider2D na aba Scene (e na Game, com Gizmos ligado).
/// Serve como "mapa técnico" dentro da Unity:
/// azul = slots, vermelho = base, amarelo = obstáculos.
/// Não interfere no jogo.
/// </summary>
public class AreaGizmo : MonoBehaviour
{
    public Color color = Color.yellow;

    void OnDrawGizmos()
    {
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        if (TryGetComponent(out BoxCollider2D box))
        {
            Gizmos.color = new Color(color.r, color.g, color.b, 0.25f);
            Gizmos.DrawCube(box.offset, box.size);
            Gizmos.color = color;
            Gizmos.DrawWireCube(box.offset, box.size);
        }

        if (TryGetComponent(out CircleCollider2D circle))
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(circle.offset, circle.radius);
        }

        Gizmos.matrix = old;
    }
}
