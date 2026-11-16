#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Monta o Tonho Defense com o mapa ilustrado, com um clique:
/// menu superior  Tonho Defense > Montar cena completa
///
/// Cria: layers, sprites, prefabs, mapa de fundo, caminho (waypoints), 4 slots de torre,
/// base do Tonho (trigger), obstáculos com colisão, GameManager e BuildManager.
/// Pode rodar de novo quando quiser: ele recria a cena e sobrescreve os prefabs.
///
/// Conversão usada: o mapa tem 1280 x 720 px e vira 17,78 x 10 unidades (72 px = 1 unidade),
/// com o centro da imagem em (0,0). Todas as posições abaixo estão em PIXELS DA IMAGEM
/// e são convertidas pelas funções Px() e PxSize().
/// </summary>
public static class TonhoSetup
{
    const string PrefabDir = "Assets/Prefabs";
    const string SpriteDir = "Assets/Sprites";
    const string ArtDir = "Assets/Art";
    const float PPU = 72f;

    // ---------------- Dados do mapa (em pixels da imagem 1280x720) ----------------

    // Caminho dos zumbis, da esquerda (fora da tela) até a casa do Tonho
    static readonly Vector2[] WaypointsPx =
    {
        new Vector2(-30, 355), new Vector2(260, 358),  new Vector2(305, 395), new Vector2(345, 448),
        new Vector2(395, 492), new Vector2(520, 502),  new Vector2(680, 502), new Vector2(738, 478),
        new Vector2(790, 420), new Vector2(832, 348),  new Vector2(950, 332), new Vector2(1022, 350),
        new Vector2(1050, 415), new Vector2(1088, 468), new Vector2(1150, 484), new Vector2(1262, 485)
    };

    // Centro das 4 bases de torre desenhadas na arte
    static readonly Vector2[] SlotsPx =
    {
        new Vector2(440, 318), new Vector2(543, 423), new Vector2(750, 325), new Vector2(898, 430)
    };

    // Obstáculos com colisão: (centro x, centro y, largura, altura) em pixels
    static readonly Vector4[] RocksPx =
    {
        new Vector4(55, 665, 130, 105), new Vector4(770, 670, 210, 100), new Vector4(212, 590, 80, 40),
        new Vector4(1085, 550, 70, 40), new Vector4(318, 552, 90, 45)
    };
    static readonly Vector4[] FencesPx =
    {
        new Vector4(55, 300, 100, 90), new Vector4(58, 385, 100, 70), new Vector4(1070, 665, 190, 90)
    };
    static readonly Vector4[] BarrelsPx = { new Vector4(290, 645, 110, 65) };
    static readonly Vector4[] PropsPx =   // carro e carroça
    {
        new Vector4(110, 548, 215, 100), new Vector4(950, 605, 125, 75)
    };

    static readonly Vector4 BaseTriggerPx = new Vector4(1235, 483, 110, 70);   // área de chegada
    static readonly Vector4 BaseColliderPx = new Vector4(1200, 332, 160, 185); // corpo da casa

    static Vector2 Px(float x, float y) { return new Vector2((x - 640f) / PPU, (360f - y) / PPU); }
    static Vector2 PxSize(float w, float h) { return new Vector2(w / PPU, h / PPU); }

    // ---------------- Montagem ----------------

    [MenuItem("Tonho Defense/Montar cena completa")]
    public static void Build()
    {
        EnsureFolder("Assets", "Prefabs");
        EnsureFolder("Assets", "Sprites");
        EnsureFolder("Assets", "Scenes");

        string mapPath = ArtDir + "/Mapa_SemTorres.png";
        if (!File.Exists(mapPath)) mapPath = ArtDir + "/Mapa_Original.jpg";
        if (!File.Exists(mapPath))
        {
            EditorUtility.DisplayDialog("Tonho Defense",
                "Não encontrei o mapa em Assets/Art (Mapa_SemTorres.png ou Mapa_Original.jpg).", "OK");
            return;
        }

        // Limpa montagens anteriores
        foreach (string oldName in new[] { "GameMap", "TonhoDefense_Cena" })
        {
            GameObject old = GameObject.Find(oldName);
            if (old != null) Object.DestroyImmediate(old);
        }

        // ---------- Layers ----------
        int lGround = EnsureLayer("Ground");
        int lPath = EnsureLayer("Path");
        int lEnemy = EnsureLayer("Enemy");
        int lTower = EnsureLayer("Tower");
        int lSlot = EnsureLayer("TowerSlot");
        int lEnv = EnsureLayer("Environment");
        int lBase = EnsureLayer("Base");
        EnsureLayer("Trigger");
        int lProj = EnsureLayer("Projectile");

        // ---------- Sprites e câmera ----------
        Sprite map = ImportMapSprite(mapPath);
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
        cam.backgroundColor = Hex("#120A1A");
        if (cam.GetComponent<CameraFit>() == null) cam.gameObject.AddComponent<CameraFit>();

        // ---------- Prefabs ----------
        Projectile bala = MakeProjectile("Bala", circle, 0.2f, "#FFEB3B", 8f, lProj);
        Projectile bola = MakeProjectile("BolaDeCanhao", circle, 0.35f, "#FF5722", 6f, lProj);

        Enemy zumbi = MakeEnemy("Zumbi", circle, 0.55f, "#4CAF50", 2f, 3, 1, 10, lEnemy);
        Enemy tanque = MakeEnemy("Tanque", circle, 0.9f, "#8B0000", 1f, 12, 3, 25, lEnemy);

        Tower torreBasica = MakeTower("TorreBasica", "Torre Básica", square, 0.8f, "#2196F3", 0.2f,
                                      50, 3f, 1, 2f, TargetMode.First, bala, lTower, lEnemy);
        Tower canhao = MakeTower("Canhao", "Canhão", square, 1.0f, "#FF9800", 0.35f,
                                 120, 4.5f, 5, 0.5f, TargetMode.Strongest, bola, lTower, lEnemy);

        // ---------- Hierarquia: GameMap ----------
        Transform gameMap = new GameObject("GameMap").transform;

        // Background (arte do mapa: separada de toda a lógica de jogo)
        Transform background = MakeGroup("Background", gameMap);
        GameObject mapGo = MakeSprite("Mapa", map, Vector2.zero, Vector2.one, "#FFFFFF", -10, background);
        mapGo.layer = lGround;

        // EnemyPath + waypoints
        GameObject pathGo = new GameObject("EnemyPath");
        pathGo.transform.SetParent(gameMap, false);
        pathGo.layer = lPath;
        EnemyPath path = pathGo.AddComponent<EnemyPath>();
        path.drawPath = false;   // o caminho de terra já está desenhado na arte
        path.waypoints = new Transform[WaypointsPx.Length];
        for (int i = 0; i < WaypointsPx.Length; i++)
        {
            GameObject wp = new GameObject($"Waypoint_{i:00}");
            wp.transform.SetParent(pathGo.transform, false);
            wp.transform.localPosition = Px(WaypointsPx[i].x, WaypointsPx[i].y);
            wp.layer = lPath;
            path.waypoints[i] = wp.transform;
        }

        // TowerSlots
        Transform slotsGroup = MakeGroup("TowerSlots", gameMap);
        for (int i = 0; i < SlotsPx.Length; i++)
        {
            GameObject slot = new GameObject($"TowerSlot_{i + 1:00}");
            slot.transform.SetParent(slotsGroup, false);
            slot.transform.localPosition = Px(SlotsPx[i].x, SlotsPx[i].y);
            slot.layer = lSlot;

            CircleCollider2D cc = slot.AddComponent<CircleCollider2D>();   // área de detecção do clique
            cc.radius = 0.7f;
            slot.AddComponent<TowerSlot>();
            slot.AddComponent<AreaGizmo>().color = new Color(0.13f, 0.59f, 0.95f);

            // Marcador azul translúcido (mostra onde dá para construir)
            GameObject marker = MakeSprite("Marcador", circle, Vector2.zero, new Vector2(1.1f, 1.1f),
                                           "#2196F359", 1, slot.transform);
            marker.layer = lSlot;
        }

        // Environment: só o que realmente tem colisão
        Transform env = MakeGroup("Environment", gameMap);
        MakeAreas("Rocks", "Rock", RocksPx, env, lEnv, new Color(1f, 0.8f, 0f));
        MakeAreas("Fences", "Fence", FencesPx, env, lEnv, new Color(1f, 0.8f, 0f));
        MakeAreas("Barrels", "Barrels", BarrelsPx, env, lEnv, new Color(1f, 0.8f, 0f));
        MakeAreas("Props", "Prop", PropsPx, env, lEnv, new Color(1f, 0.8f, 0f));
        MakeGroup("Decorations", env);   // nuvens, lua, cidade, vegetação: SEM colisão (fica vazio de propósito)

        // PlayerBase: BaseTrigger (chegada + vida) e BaseCollider (corpo da casa)
        Transform playerBase = MakeGroup("PlayerBase", gameMap);
        playerBase.gameObject.layer = lBase;

        GameObject baseTrigger = MakeArea("BaseTrigger", playerBase, BaseTriggerPx, lBase,
                                          new Color(1f, 0.2f, 0.2f), true);
        baseTrigger.AddComponent<BaseTonho>();     // OnTriggerEnter2D precisa estar neste objeto
        MakeArea("BaseCollider", playerBase, BaseColliderPx, lEnv, new Color(1f, 0.2f, 0.2f), false);

        // Gerentes
        Transform managers = MakeGroup("Managers", gameMap);

        GameObject gmGo = new GameObject("GameManager");
        gmGo.transform.SetParent(managers, false);
        WaveSpawner ws = gmGo.AddComponent<WaveSpawner>();
        ws.path = path;
        ws.waves = new Wave[]
        {
            MakeWave("Onda 1", 5f, Grp(zumbi, 5, 1.5f)),
            MakeWave("Onda 2", 5f, Grp(zumbi, 6, 1.2f), Grp(tanque, 2, 3f)),
            MakeWave("Onda 3", 0f, Grp(zumbi, 8, 1f), Grp(tanque, 3, 2.5f))
        };
        GameManager gm = gmGo.AddComponent<GameManager>();
        gm.spawner = ws;
        gm.timeLimit = 180f;
        gm.killsToWin = 10;
        gm.startingMoney = 150;

        GameObject bmGo = new GameObject("BuildManager");
        bmGo.transform.SetParent(managers, false);
        BuildManager bm = bmGo.AddComponent<BuildManager>();
        bm.towerPrefabs = new Tower[] { torreBasica, canhao };
        bm.cam = cam;
        bm.slotLayers = 1 << lSlot;

        // Matriz de colisão (também aplicada em runtime pelo GameManager)
        PhysicsLayers.Apply();

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

        Selection.activeGameObject = gameMap.gameObject;
        EditorUtility.DisplayDialog("Tonho Defense", "Cena montada! Aperte o botão Play.", "OK");
    }

    // ---------------- Helpers ----------------

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

    /// <summary>Cria a layer pelo nome (no TagManager) se ainda não existir. Retorna o índice.</summary>
    static int EnsureLayer(string layerName)
    {
        int existing = LayerMask.NameToLayer(layerName);
        if (existing >= 0) return existing;

        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        for (int i = 8; i < layers.arraySize; i++)   // 0-7 são reservadas pela Unity
        {
            SerializedProperty sp = layers.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(sp.stringValue))
            {
                sp.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                return i;
            }
        }

        Debug.LogWarning("Sem espaço para criar a layer " + layerName);
        return 0;
    }

    static void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursive(child.gameObject, layer);
    }

    static Sprite ImportMapSprite(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter imp = (TextureImporter)AssetImporter.GetAtPath(path);
        imp.textureType = TextureImporterType.Sprite;
        imp.spriteImportMode = SpriteImportMode.Single;
        imp.spritePixelsPerUnit = PPU;          // 72 px = 1 unidade -> 1280x720 vira 17,78 x 10
        imp.mipmapEnabled = false;
        imp.maxTextureSize = 2048;
        imp.textureCompression = TextureImporterCompression.Uncompressed;
        imp.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
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

    static Transform MakeGroup(string name, Transform parent)
    {
        GameObject g = new GameObject(name);
        g.transform.SetParent(parent, false);
        return g.transform;
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

    /// <summary>Objeto invisível só com BoxCollider2D (a arte fica separada das colisões).</summary>
    static GameObject MakeArea(string name, Transform parent, Vector4 px, int layer, Color gizmoColor, bool trigger)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = Px(px.x, px.y);
        go.layer = layer;

        BoxCollider2D box = go.AddComponent<BoxCollider2D>();
        box.size = PxSize(px.z, px.w);
        box.isTrigger = trigger;

        go.AddComponent<AreaGizmo>().color = gizmoColor;
        return go;
    }

    static void MakeAreas(string groupName, string itemName, Vector4[] items, Transform parent,
                          int layer, Color gizmoColor)
    {
        Transform group = MakeGroup(groupName, parent);
        for (int i = 0; i < items.Length; i++)
            MakeArea($"{itemName}_{i + 1:00}", group, items[i], layer, gizmoColor, false);
    }

    static GameObject SavePrefab(GameObject go, string name)
    {
        string path = $"{PrefabDir}/{name}.prefab";
        GameObject asset = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return asset;
    }

    static Projectile MakeProjectile(string name, Sprite circle, float size, string hex, float speed, int layer)
    {
        GameObject go = MakeSprite(name, circle, Vector2.zero, new Vector2(size, size), hex, 6, null);
        go.layer = layer;

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
                           float speed, int hp, int damage, int reward, int layer)
    {
        GameObject go = MakeSprite(name, circle, Vector2.zero, new Vector2(size, size), hex, 3, null);
        go.layer = layer;

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
                           float fireRate, TargetMode mode, Projectile projectile,
                           int towerLayer, int enemyLayer)
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
        t.targetLayers = 1 << enemyLayer;   // a torre só enxerga a layer Enemy

        SetLayerRecursive(root, towerLayer);

        return SavePrefab(root, name).GetComponent<Tower>();
    }

    static SpawnGroup Grp(Enemy prefab, int count, float interval)
    {
        return new SpawnGroup { prefab = prefab, count = count, interval = interval };
    }

    static Wave MakeWave(string name, float delayAfter, params SpawnGroup[] groups)
    {
        return new Wave { name = name, groups = groups, delayAfter = delayAfter };
    }
}
#endif
