using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _horizontalDolly, _verticalDolly, _anglePivot;
    
    [SerializeField]
    private Vector2 _horizontalDollyRange, _verticalDollyRange;
    
    private IEnumerator Start()
    {
        while (!GameManager.Instance.isInitialized)
            yield return null;

        GameManager.Instance.kaijuController.OnCoinCollected += HandleCoinCollected;
    }

    private void HandleCoinCollected(int coins)
    {
        var percentage = GameManager.Instance.kaijuController.CoinsToNormalizedValue(coins);
        
        var z = Mathf.Lerp(_horizontalDollyRange.x, _horizontalDollyRange.y, percentage);
        var y = Mathf.Lerp(_verticalDollyRange.x, _verticalDollyRange.y, percentage);

        _horizontalDolly.DOLocalMoveZ(z, 0.25f);
        _verticalDolly.DOLocalMoveY(y, 0.25f);
    }
}
