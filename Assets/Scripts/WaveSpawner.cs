using System.Collections;
using UnityEngine;

/// <summary>Um grupo de zumbis do mesmo tipo dentro de uma onda.</summary>
[System.Serializable]
public class SpawnGroup
{
    public Enemy prefab;
    public int count = 5;
    public float interval = 1.2f;   // segundos entre um zumbi e outro
}

/// <summary>Uma onda: vários grupos que nascem em sequência.</summary>
[System.Serializable]
public class Wave
{
    public string name = "Onda";
    public SpawnGroup[] groups;
    public float delayAfter = 5f;   // pausa antes da próxima onda
}

/// <summary>
/// Cria os zumbis no primeiro waypoint do caminho, onda por onda.
/// É iniciado pelo GameManager quando a partida começa.
/// </summary>
public class WaveSpawner : MonoBehaviour
{
    public EnemyPath path;
    public Wave[] waves;
    public float firstWaveDelay = 2f;

    /// <summary>True quando todos os zumbis de todas as ondas já nasceram.</summary>
    public bool Finished { get; private set; }
    public int CurrentWave { get; private set; }

    public void Begin()
    {
        StopAllCoroutines();
        Finished = false;
        CurrentWave = 0;
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        yield return new WaitForSeconds(firstWaveDelay);

        for (int w = 0; w < waves.Length; w++)
        {
            CurrentWave = w + 1;

            foreach (SpawnGroup group in waves[w].groups)
            {
                for (int i = 0; i < group.count; i++)
                {
                    Spawn(group.prefab);
                    yield return new WaitForSeconds(group.interval);
                }
            }

            yield return new WaitForSeconds(waves[w].delayAfter);
        }

        Finished = true;
    }

    void Spawn(Enemy prefab)
    {
        Enemy enemy = Instantiate(prefab, path.waypoints[0].position, Quaternion.identity);
        enemy.Init(path);
        GameManager.Instance.RegisterSpawn();
    }
}
