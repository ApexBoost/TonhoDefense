using UnityEngine;

/// <summary>
/// Caminho dos zumbis: um array de waypoints (pontos pelos quais eles passam, em ordem).
/// Como montar: crie um objeto vazio "Caminho", adicione este script e crie filhos vazios
/// P0, P1, P2... A ordem dos filhos na Hierarchy é a ordem do caminho.
/// Se o array ficar vazio, ele é preenchido sozinho com os filhos.
/// </summary>
public class EnemyPath : MonoBehaviour
{
    public Transform[] waypoints;

    [Header("Visual do caminho (só estética)")]
    public bool drawPath = true;
    public float pathWidth = 0.9f;
    public Color pathColor = new Color(0.45f, 0.30f, 0.17f); // marrom mais claro que o chão

    void Awake()
    {
        if (waypoints == null || waypoints.Length == 0) waypoints = GetChildren();
        if (drawPath) DrawLine();
    }

    Transform[] GetChildren()
    {
        Transform[] list = new Transform[transform.childCount];
        for (int i = 0; i < list.Length; i++) list[i] = transform.GetChild(i);
        return list;
    }

    // Desenha uma faixa por cima do chão para o jogador enxergar o caminho
    void DrawLine()
    {
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = pathColor;
        lr.endColor = pathColor;
        lr.startWidth = pathWidth;
        lr.endWidth = pathWidth;
        lr.numCornerVertices = 6;
        lr.numCapVertices = 6;
        lr.sortingOrder = -5; // acima do chão (-10), abaixo de tudo
        lr.useWorldSpace = true;
        lr.positionCount = waypoints.Length;
        for (int i = 0; i < waypoints.Length; i++)
            lr.SetPosition(i, waypoints[i].position);
    }

    // Mostra o caminho na aba Scene (linha amarela) para facilitar o ajuste
    void OnDrawGizmos()
    {
        Transform[] pts = (waypoints != null && waypoints.Length > 0) ? waypoints : GetChildren();
        Gizmos.color = Color.yellow;
        for (int i = 0; i < pts.Length; i++)
        {
            if (pts[i] == null) continue;
            Gizmos.DrawWireSphere(pts[i].position, 0.15f);
            if (i < pts.Length - 1 && pts[i + 1] != null)
                Gizmos.DrawLine(pts[i].position, pts[i + 1].position);
        }
    }
}
