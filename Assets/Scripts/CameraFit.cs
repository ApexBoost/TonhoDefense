using UnityEngine;

/// <summary>
/// Ajusta o zoom da câmera ortográfica para o mapa inteiro caber em qualquer proporção de tela
/// (16:9, 16:10, ultrawide, celular...). O mapa tem 17,78 x 10 unidades.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraFit : MonoBehaviour
{
    public float worldWidth = 17.78f;
    public float worldHeight = 10f;

    Camera cam;

    void Awake() { cam = GetComponent<Camera>(); }

    void Update()
    {
        float sizeByHeight = worldHeight / 2f;
        float sizeByWidth = worldWidth / cam.aspect / 2f;
        cam.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);
    }
}
