using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Lê o mouse tanto no Input System novo quanto no antigo.
/// Assim o projeto funciona sem precisar mexer nas configurações da Unity.
/// </summary>
public static class InputHelper
{
    /// <summary>True no frame em que o botão esquerdo foi apertado.</summary>
    public static bool ClickDown()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    /// <summary>Posição do mouse em pixels de tela.</summary>
    public static Vector2 PointerScreenPosition()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }
}
