using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _battleCamera, _menuCamera;
    
    private KaijuController _kaijuController;
    private bool _initialized;
    
    private IEnumerator Start()
    {
        while (!GameManager.Instance.isInitialized)
            yield return null;

        _initialized = true;
        _kaijuController = GameManager.Instance.kaijuController;
        
        _battleCamera.gameObject.SetActive(_isBattleMode);
        _menuCamera.gameObject.SetActive(!_isBattleMode);
    }

    private void Update()
    {
        if (!_initialized)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            ToggleMode();
    }

    private bool _isBattleMode = true;
    private void ToggleMode()
    {
        _isBattleMode = !_isBattleMode;

        _battleCamera.gameObject.SetActive(_isBattleMode);
        _menuCamera.gameObject.SetActive(!_isBattleMode);
    }
}
