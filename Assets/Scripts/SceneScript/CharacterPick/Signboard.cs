using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Signboard : MonoBehaviour
{
    bool[] _isDisplaySignboard = new bool[2] { false, false };

    // 
    public void ToggleDoDisplaySignboard(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        var id = Identification(ctx);
        _isDisplaySignboard[id] = !_isDisplaySignboard[id];
    }

    protected virtual int Identification(InputAction.CallbackContext ctx)
    {
        // Ž¯•Ê
        if (Gamepad.all[0] == ctx.control.device) return 0;
        else return 1;
    }
}