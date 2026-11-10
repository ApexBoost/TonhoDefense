using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, Won, Lost }

/// <summary>
/// Controla a partida: estado do jogo, tempo máximo, dinheiro, abates, vitória e derrota.
/// Também desenha o menu, o HUD e as telas de fim com OnGUI (sem Canvas).
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Regras da partida")]
    public float timeLimit = 180f;      // tempo máximo da partida (segundos)
    public int killsToWin = 10;         // zumbis que precisam morrer para vencer (mínimo do professor: 5)
    public int startingMoney = 150;

    [Header("Referências")]
    public WaveSpawner spawner;

    public GameState State { get; private set; } = GameState.Menu;
    public float TimeRemaining { get; private set; }
    public int Kills { get; private set; }
    public int Money { get; private set; }
    public int AliveEnemies { get; private set; }
    public string EndReason { get; private set; } = "";

    // Para o botão "Jogar novamente" pular o menu depois de recarregar a cena
    static bool autoStartAfterRestart;

    GUIStyle labelStyle, titleStyle, textStyle, buttonStyle;

    // ---------- Ciclo de vida ----------

    void Awake()
    {
        Instance = this;
        Time.timeScale = 0f;    // jogo pausado até apertar "Jogar"
    }

    void Start()
    {
        if (autoStartAfterRestart)
        {
            autoStartAfterRestart = false;
            StartGame();
        }
    }

    public void StartGame()
    {
        State = GameState.Playing;
        TimeRemaining = timeLimit;
        Money = startingMoney;
        Kills = 0;
        AliveEnemies = 0;
        Time.timeScale = 1f;
        spawner.Begin();
    }

    void Update()
    {
        if (State != GameState.Playing) return;

        // Tempo máximo de partida
        TimeRemaining -= Time.deltaTime;
        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            Lose("O tempo da missão acabou!");
            return;
        }

        // Todas as ondas já nasceram e não sobrou nenhum zumbi vivo
        if (spawner.Finished && AliveEnemies <= 0)
        {
            if (Kills >= killsToWin) Win($"Você sobreviveu a todas as ondas!");
            else Lose("Acabaram as ondas e você não eliminou zumbis suficientes.");
        }
    }

    // ---------- Eventos chamados por outros scripts ----------

    public void RegisterSpawn() { AliveEnemies++; }

    public void EnemyKilled(int reward)
    {
        AliveEnemies--;
        Kills++;
        Money += reward;
        if (Kills >= killsToWin) Win($"Você eliminou {Kills} zumbis!");
    }

    public void EnemyReachedBase() { AliveEnemies--; }

    public bool TrySpend(int cost)
    {
        if (Money < cost) return false;
        Money -= cost;
        return true;
    }

    public void Win(string reason) { EndGame(GameState.Won, reason); }
    public void Lose(string reason) { EndGame(GameState.Lost, reason); }

    void EndGame(GameState result, string reason)
    {
        if (State != GameState.Playing) return;   // evita terminar duas vezes
        State = result;
        EndReason = reason;
        Time.timeScale = 0f;                      // congela o jogo
    }

    // ---------- Interface (OnGUI) ----------

    // A GUI usa uma altura "virtual" de 720 pixels e escala para qualquer resolução
    public static float GuiScale => Screen.height / 720f;

    /// <summary>Converte posição do mouse (pixels) para a coordenada virtual da GUI.</summary>
    public static Vector2 ScreenToGui(Vector2 screenPos)
    {
        return new Vector2(screenPos.x / GuiScale, (Screen.height - screenPos.y) / GuiScale);
    }

    void BuildStyles()
    {
        if (labelStyle != null) return;

        labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold };
        labelStyle.normal.textColor = Color.white;

        titleStyle = new GUIStyle(labelStyle) { fontSize = 64, alignment = TextAnchor.MiddleCenter };

        textStyle = new GUIStyle(labelStyle)
        {
            fontSize = 24,
            fontStyle = FontStyle.Normal,
            alignment = TextAnchor.MiddleCenter
        };

        buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 28, fontStyle = FontStyle.Bold };
    }

    void OnGUI()
    {
        BuildStyles();

        float s = GuiScale;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1f));
        float vw = Screen.width / s;   // largura virtual

        if (State == GameState.Playing) DrawHud(vw);
        else DrawOverlay(vw);
    }

    void DrawHud(float vw)
    {
        GUI.Box(new Rect(10, 10, vw - 20, 44), GUIContent.none);

        int life = BaseTonho.Instance != null ? BaseTonho.Instance.Life : 0;
        int maxLife = BaseTonho.Instance != null ? BaseTonho.Instance.maxLife : 0;
        int min = Mathf.FloorToInt(TimeRemaining / 60f);
        int sec = Mathf.FloorToInt(TimeRemaining % 60f);

        GUI.Label(new Rect(24, 14, vw - 40, 36),
            $"Base: {life}/{maxLife}     Tempo: {min:00}:{sec:00}     Dinheiro: ${Money}     " +
            $"Abates: {Kills}/{killsToWin}     Onda: {spawner.CurrentWave}/{spawner.waves.Length}",
            labelStyle);
    }

    void DrawOverlay(float vw)
    {
        // Fundo escurecido
        GUI.color = new Color(0f, 0f, 0f, 0.75f);
        GUI.DrawTexture(new Rect(0, 0, vw, 720), Texture2D.whiteTexture);
        GUI.color = Color.white;

        float cx = vw / 2f;

        if (State == GameState.Menu)
        {
            titleStyle.normal.textColor = new Color(1f, 0.6f, 0.1f);
            GUI.Label(new Rect(cx - 500, 120, 1000, 90), "TONHO DEFENSE", titleStyle);
            GUI.Label(new Rect(cx - 500, 230, 1000, 40), "Defenda a Base do Tonho dos zumbis!", textStyle);
            GUI.Label(new Rect(cx - 500, 280, 1000, 80),
                "Escolha a torre na barra de baixo e clique em um quadrado cinza para construir.", textStyle);

            if (GUI.Button(new Rect(cx - 150, 390, 300, 70), "JOGAR (Fácil)", buttonStyle))
                StartGame();
        }
        else
        {
            bool won = State == GameState.Won;
            titleStyle.normal.textColor = won ? new Color(0.3f, 1f, 0.4f) : new Color(1f, 0.3f, 0.3f);
            GUI.Label(new Rect(cx - 500, 150, 1000, 90), won ? "VITÓRIA!" : "GAME OVER", titleStyle);
            GUI.Label(new Rect(cx - 500, 260, 1000, 40), EndReason, textStyle);
            GUI.Label(new Rect(cx - 500, 310, 1000, 40), $"Zumbis eliminados: {Kills}", textStyle);

            if (GUI.Button(new Rect(cx - 150, 400, 300, 70), "Jogar novamente", buttonStyle))
            {
                autoStartAfterRestart = true;
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
