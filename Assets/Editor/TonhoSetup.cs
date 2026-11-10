#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Monta o Tonho Defense inteiro com um clique:
/// menu superior  Tonho Defense > Montar cena completa
/// Cria sprites, prefabs (zumbi, tanque, torres, projéteis), cenário, caminho, base,
/// slots, GameManager e BuildManager, já com todas as referências ligadas.
/// Pode rodar de novo a qualquer momento: ele recria a cena e sobrescreve os prefabs.
/// </summary>
public static class TonhoSetup
{
    const string PrefabDir = "Assets/Prefabs";
    const string SpriteDir = "Assets/Sprites";
    const string RootName = "TonhoDefense_Cena";

    [MenuItem("Tonho Defense/Montar cena completa")]
    public static void Build()
    {
        EnsureFolder("Assets", "Prefabs");
        EnsureFolder("Assets", "Sprites");
        EnsureFolder("Assets", "Scenes");

        // Limpa uma montagem anterior, se existir
        GameObject old = GameObject.Find(RootName);
        if (old != null) Object.DestroyImmediate(old);

        // ---------- Sprites e câmera ----------
        Sprite square = CreateSprite("Quadrado", false);
        Sprite circle = CreateSprite("Circulo", true);

        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            cam = camGo.AddComponent<Camera>();
        }
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Hex("#1A0D24");

        // ---------- Prefabs ----------
        Projectile bala = MakeProjectile("Bala", circle, 0.2f, "#FFEB3B", 8f);
        Projectile bola = MakeProjectile("BolaDeCanhao", circle, 0.35f, "#FF5722", 6f);

        Enemy zumbi = MakeEnemy("Zumbi", circle, 0.5f, "#4CAF50", 2f, 3, 1, 10);
        Enemy tanque = MakeEnemy("Tanque", circle, 0.9f, "#8B0000", 1f, 12, 3, 25);

        Tower torreBasica = MakeTower("TorreBasica", "Torre Básica", square, 0.7f, "#2196F3", 0.2f,
                                      50, 3f, 1, 2f, TargetMode.First, bala);
        Tower canhao = MakeTower("Canhao", "Canhão", square, 0.9f, "#FF9800", 0.35f,
                                 120, 4.5f, 5, 0.5f, TargetMode.Strongest, bola);

        // ---------- Cena ----------
        Transform root = new GameObject(RootName).transform;

        MakeSprite("Ceu", square, new Vector2(0f, 2.5f), new Vector2(40f, 5f), "#2B1240", -10, root);
        MakeSprite("Chao", square, new Vector2(0f, -2.5f), new Vector2(40f, 5f), "#6B4226", -10, root);

        // Caminho com curvas (8 waypoints)
        GameObject pathGo = new GameObject("Caminho");
        pathGo.transform.SetParent(root, false);
        EnemyPath path = pathGo.AddComponent<EnemyPath>();
        Vector2[] pts =
        {
            new Vector2(-8.5f, -1.2f), new Vector2(-4.5f, -1.2f), new Vector2(-4.5f, -3.6f),
            new Vector2(0f, -3.6f),    new Vector2(0f, -1.2f),    new Vector2(4.5f, -1.2f),
            new Vector2(4.5f, -3.6f),  new Vector2(7.4f, -3.6f)
        };
        path.waypoints = new Transform[pts.Length];
        for (int i = 0; i < pts.Length; i++)
        {
            GameObject p = new GameObject("P" + i);
            p.transform.SetParent(pathGo.transform, false);
            p.transform.localPosition = pts[i];
            path.waypoints[i] = p.transform;
        }

        // Base do Tonho
        GameObject baseGo = MakeSprite("BaseTonho", square, new Vector2(7.6f, -3.6f), new Vector2(1.4f, 1.6f), "#2ECC71", 2, root);
        BoxCollider2D baseCol = baseGo.AddComponent<BoxCollider2D>();
        baseCol.size = Vector2.one;
        baseCol.isTrigger = true;
        baseGo.AddComponent<BaseTonho>();

        // Slots de construção
        Vector2[] slots =
        {
            new Vector2(-6.5f, -2.6f), new Vector2(-2.2f, -2.2f), new Vector2(-2.2f, -0.5f),
            new Vector2(2.2f, -2.4f),  new Vector2(6f, -1.8f),    new Vector2(6f, -0.6f)
        };
        for (int i = 0; i < slots.Length; i++)
        {
            GameObject s = MakeSprite("Slot" + (i + 1), square, slots[i], new Vector2(0.9f, 0.9f), "#9E9E9E", 1, root);
            BoxCollider2D c = s.AddComponent<BoxCollider2D>();
            c.size = Vector2.one;
            s.AddComponent<TowerSlot>();
        }

        // GameManager + WaveSpawner
        GameObject gmGo = new GameObject("GameManager");
        gmGo.transform.SetParent(root, false);

        WaveSpawner ws = gmGo.AddComponent<WaveSpawner>();
        ws.path = path;
        ws.waves = new Wave[]
        {
            MakeWave("Onda 1", 5f, Group(zumbi, 5, 1.5f)),
            MakeWave("Onda 2", 5f, Group(zumbi, 6, 1.2f), Group(tanque, 2, 3f)),
            MakeWave("Onda 3", 0f, Group(zumbi, 8, 1f), Group(tanque, 3, 2.5f))
        };

        GameManager gm = gmGo.AddComponent<GameManager>();
        gm.spawner = ws;
        gm.timeLimit = 180f;
        gm.killsToWin = 10;
        gm.startingMoney = 150;

        // BuildManager
        GameObject bmGo = new GameObject("BuildManager");
        bmGo.transform.SetParent(root, false);
        BuildManager bm = bmGo.AddComponent<BuildManager>();
        bm.towerPrefabs = new Tower[] { torreBasica, canhao };
        bm.cam = cam;

        // ---------- Salvar cena e registrar no Build Settings ----------
        AssetDatabase.SaveAssets();
        Scene scene = SceneManager.GetActiveScene();
        if (string.IsNullOrEmpty(scene.path))
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainScene.unity");
        else
            EditorSceneManager.SaveScene(scene);

        var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        if (!list.Exists(s => s.path == scene.path))
        {
            list.Add(new EditorBuildSettingsScene(scene.path, true));
            EditorBuildSettings.scenes = list.ToArray();
        }

        Selection.activeGameObject = root.gameObject;
        EditorUtility.DisplayDialog("Tonho Defense", "Cena montada! Aperte o botão Play.", "OK");
    }

    // ================= Helpers =================

    static void EnsureFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + name))
            AssetDatabase.CreateFolder(parent, name);
    }

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color c);
        return c;
    }

    /// <summary>Gera um PNG branco (quadrado ou círculo) e importa como Sprite de 1x1 unidade.</summary>
    static Sprite CreateSprite(string fileName, bool circle)
    {
        string path = $"{SpriteDir}/{fileName}.png";

        if (!File.Exists(path))
        {
            const int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float r = size / 2f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - (size - 1) / 2f;
                    float dy = y - (size - 1) / 2f;
                    bool inside = !circle || (dx * dx + dy * dy <= r * r);
                    tex.SetPixel(x, y, inside ? Color.white : new Color(1f, 1f, 1f, 0f));
                }
            }
            tex.Apply();
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        }

        TextureImporter imp = (TextureImporter)AssetImporter.GetAtPath(path);
        imp.textureType = TextureImporterType.Sprite;
        imp.spriteImportMode = SpriteImportMode.Single;
        imp.spritePixelsPerUnit = 128;      // 128 px = 1 unidade
        imp.alphaIsTransparency = true;
        imp.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static GameObject MakeSprite(string name, Sprite sprite, Vector2 pos, Vector2 scale,
                                 string hex, int order, Transform parent)
    {
        GameObject go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
        go.transform.localScale = new Vector3(scale.x, scale.y, 1f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = Hex(hex);
        sr.sortingOrder = order;
        sr.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
        return go;
    }

    static GameObject SavePrefab(GameObject go, string name)
    {
        string path = $"{PrefabDir}/{name}.prefab";
        GameObject asset = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return asset;
    }

    static Projectile MakeProjectile(string name, Sprite circle, float size, string hex, float speed)
    {
        GameObject go = MakeSprite(name, circle, Vector2.zero, new Vector2(size, size), hex, 6, null);

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.8f;          // um pouco maior que o desenho, para acertar mais fácil
        col.isTrigger = true;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        Projectile p = go.AddComponent<Projectile>();
        p.speed = speed;

        return SavePrefab(go, name).GetComponent<Projectile>();
    }

    static Enemy MakeEnemy(string name, Sprite circle, float size, string hex,
                           float speed, int hp, int damage, int reward)
    {
        GameObject go = MakeSprite(name, circle, Vector2.zero, new Vector2(size, size), hex, 3, null);

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.5f;          // NÃO é trigger

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        Enemy e = go.AddComponent<Enemy>();
        e.speed = speed;
        e.maxHealth = hp;
        e.damageToBase = damage;
        e.reward = reward;

        return SavePrefab(go, name).GetComponent<Enemy>();
    }

    static Tower MakeTower(string name, string label, Sprite square, float bodySize, string hex,
                           float barrelThickness, int cost, float range, int damage,
                           float fireRate, TargetMode mode, Projectile projectile)
    {
        GameObject root = new GameObject(name);

        MakeSprite("Corpo", square, Vector2.zero, new Vector2(bodySize, bodySize), hex, 5, root.transform);

        // Cano: aponta para o eixo X (a torre inteira gira para mirar)
        float half = bodySize / 2f;
        MakeSprite("Cano", square, new Vector2(half, 0f), new Vector2(0.5f, barrelThickness), "#FFFFFF", 6, root.transform);

        GameObject fp = new GameObject("FirePoint");
        fp.transform.SetParent(root.transform, false);
        fp.transform.localPosition = new Vector3(half + 0.25f, 0f, 0f);

        Tower t = root.AddComponent<Tower>();
        t.towerName = label;
        t.cost = cost;
        t.range = range;
        t.damage = damage;
        t.fireRate = fireRate;
        t.targetMode = mode;
        t.projectilePrefab = projectile;
        t.firePoint = fp.transform;

        return SavePrefab(root, name).GetComponent<Tower>();
    }

    static SpawnGroup Group(Enemy prefab, int count, float interval)
    {
        return new SpawnGroup { prefab = prefab, count = count, interval = interval };
    }

    static Wave MakeWave(string name, float delayAfter, params SpawnGroup[] groups)
    {
        return new Wave { name = name, groups = groups, delayAfter = delayAfter };
    }
}
#endif
