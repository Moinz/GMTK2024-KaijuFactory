using System;
using DG.Tweening;
using UnityEngine;

public class KaijuController : MonoBehaviour
{
    public Action<int> OnCoinCollected;

    public Transform kaijuPivotTransform;

    [SerializeField]
    private Vector2 kaijuMinMaxScale = new(1f, 10f);

    [Range(3, 100)]
    public int coinsToCollect = 15;

    private int _coinsCollected;

    private void OnEnable()
    {
        OnCoinCollected += HandleOnCoinCollected;
    }

    private void OnDisable()
    {
        OnCoinCollected -= HandleOnCoinCollected;
    }

    private void OnCollisionEnter(Collision other)
    {
        var otherRigidbody = other.rigidbody;
        
        if (!otherRigidbody)
            return;
        
        if (!otherRigidbody.CompareTag("Coin"))
            return;

        _coinsCollected++;
        OnCoinCollected?.Invoke(_coinsCollected);
        
        ResetCoin(otherRigidbody);
    }
    
    private void HandleOnCoinCollected(int coinsCollected)
    {
        Scale(coinsCollected);
    }
    
    private void Scale(int coinsCollected)
    {
        coinsCollected = Mathf.Max(coinsCollected, 1);

        var scale = CoinsToScale(coinsCollected);

        kaijuPivotTransform.DOScale(Vector3.one * scale, 0.25f).SetEase(Ease.InOutBounce);
    }

    public float CoinsToScale(int coins)
    {
        float coinsToPercentage = (float) coins /  coinsToCollect;
        float scale = Mathf.Lerp(kaijuMinMaxScale.x, kaijuMinMaxScale.y, coinsToPercentage);

        return scale;
    }

    public float CoinsToNormalizedValue(int coins)
    {
        float coinsToPercentage = (float) coins /  coinsToCollect;

        return coinsToPercentage;
    }

    private void ResetCoin(Rigidbody coinRB)
    {
        coinRB.gameObject.SetActive(false);
        coinRB.transform.position += Vector3.up * CoinsToScale(_coinsCollected) * 1.5f;
        coinRB.velocity = Vector3.zero;
        coinRB.angularVelocity = Vector3.zero;
        
        coinRB.gameObject.SetActive(true);
    }
}