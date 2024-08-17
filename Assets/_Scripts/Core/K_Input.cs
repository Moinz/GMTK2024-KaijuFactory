using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class K_Input : KaijuControls.IKaijuActionActions
{
    [SerializeField]
    private InputActionReference walkLeftAction, walkRightAction;

    public Action OnWalkLeftInput;
    public Action OnWalkRightInput;
    
    public void Init(Action onWalkLeftInput, Action onWalkRightInput)
    {
        OnWalkLeftInput = onWalkLeftInput;
        OnWalkRightInput = onWalkRightInput;
        
        KaijuControls kaijuControls = new KaijuControls();
        
        kaijuControls.Enable();
        kaijuControls.KaijuAction.AddCallbacks(this);
        kaijuControls.KaijuAction.Enable();
    }
    
    public void OnWalkLeft(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        
        OnWalkLeftInput?.Invoke();
    }

    public void OnWalkRight(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        
        OnWalkRightInput?.Invoke();
    }
}