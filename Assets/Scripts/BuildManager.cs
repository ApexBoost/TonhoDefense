using UnityEngine;

/// <summary>
/// Controla a construção de torres: o jogador escolhe o tipo na barra de baixo
/// e clica em um TowerSlot livre. Gasta dinheiro do GameManager.
/// A barra é desenhada com OnGUI, então não precisa criar Canvas.
/// </summary>
public class BuildManager : MonoBehaviour
{
    [Tooltip("Prefabs das torres, na ordem dos botões (ex.: Torre Básica, Canhão)")]
    public Tower[] towerPrefabs;
    public Camera cam;

    int selected;
    string message = "";
    float messageUntil;
    GUIStyle buttonStyle, messageStyle;

    // Retângulo da barra (em coordenadas "virtuais" de GUI, altura fixa de 720)
    Rect BarRect
    {
        get
        {
            int n = towerPrefabs != null ? towerPrefabs.Length : 0;
            return new Rect(10, 650, 20 + n * 255, 60);
        }
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.State != GameState.Playing) return;
        if (!InputHelper.ClickDown()) return;

        Vector2 screenPos = InputHelper.PointerScreenPosition();

        // Clique na barra de botões não constrói nada
        if (BarRect.Contains(GameManager.ScreenToGui(screenPos))) return;

        Camera c = cam != null ? cam : Camera.main;
        Vector2 worldPos = c.ScreenToWorldPoint(screenPos);

        // Procura um TowerSlot livre sob o mouse
        foreach (Collider2D col in Physics2D.OverlapPointAll(worldPos))
        {
            if (col.TryGetComponent(out TowerSlot slot) && !slot.IsOccupied)
            {
                TryBuild(slot);
                return;
            }
        }
    }

    void TryBuild(TowerSlot slot)
    {
        if (towerPrefabs == null || towerPrefabs.Length == 0) return;

        Tower prefab = towerPrefabs[selected];

        if (!GameManager.Instance.TrySpend(prefab.cost))
        {
            ShowMessage("Dinheiro insuficiente!");
            return;
        }

        Instantiate(prefab, slot.transform.position, Quaternion.identity);
        slot.Occupy();
    }

    void ShowMessage(string text)
    {
        message = text;
        messageUntil = Time.time + 1.5f;
    }

    void OnGUI()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.State != GameState.Playing) return;
        if (towerPrefabs == null || towerPrefabs.Length == 0) return;

        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 20, fontStyle = FontStyle.Bold };
            messageStyle = new GUIStyle(GUI.skin.label) { fontSize = 24, fontStyle = FontStyle.Bold };
            messageStyle.normal.textColor = Color.yellow;
        }

        float s = GameManager.GuiScale;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1f));

        GUI.Box(BarRect, GUIContent.none);

        for (int i = 0; i < towerPrefabs.Length; i++)
        {
            Tower t = towerPrefabs[i];
            Rect r = new Rect(20 + i * 255, 658, 245, 44);

            GUI.backgroundColor = (i == selected) ? Color.yellow : Color.white;
            if (GUI.Button(r, $"{t.towerName}  (${t.cost})", buttonStyle))
                selected = i;
        }
        GUI.backgroundColor = Color.white;

        if (Time.time < messageUntil)
            GUI.Label(new Rect(20, 610, 500, 36), message, messageStyle);
    }
}
