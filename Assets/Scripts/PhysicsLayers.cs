using UnityEngine;

/// <summary>
/// Configura a matriz de colisões do Physics 2D por código
/// (equivale a Edit > Project Settings > Physics 2D > Layer Collision Matrix).
/// Desliga todas as combinações e liga só as necessárias:
///   Enemy x Projectile : o tiro acerta o zumbi
///   Enemy x Base       : o zumbi chega na casa do Tonho
/// Torres, slots e cenário não precisam colidir entre si:
/// a mira e o clique usam consultas (OverlapCircle / OverlapPoint) com LayerMask.
/// </summary>
public static class PhysicsLayers
{
    public static void Apply()
    {
        int enemy = LayerMask.NameToLayer("Enemy");
        int projectile = LayerMask.NameToLayer("Projectile");
        int baseLayer = LayerMask.NameToLayer("Base");

        // Layers ainda não criadas? Mantém a matriz padrão.
        if (enemy < 0 || projectile < 0 || baseLayer < 0) return;

        for (int i = 0; i < 32; i++)
            for (int j = i; j < 32; j++)
                Physics2D.IgnoreLayerCollision(i, j, true);

        Physics2D.IgnoreLayerCollision(enemy, projectile, false);
        Physics2D.IgnoreLayerCollision(enemy, baseLayer, false);
    }
}
