using System;
using DG.Tweening;
using UnityEngine;

public class KaijuController : MonoBehaviour
{
    public Action<int> OnCoinCollected;

    public K_Input input;
    public Transform kaijuPivotTransform;

    [SerializeField]
    private Vector2 kaijuMinMaxScale = new(1f, 10f);

    [Range(3, 100)]
    public int coinsToCollect = 15;

    private int _coinsCollected;

    [SerializeField]
    private float stepDistance = 0.5f;

    private void Start()
    {
        input.Init(OnWalkLeft, OnWalkRight);
    }

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
    
    private void OnWalkRight()
    {
        Vector3 newPosition = kaijuPivotTransform.position;
        newPosition += new Vector3(.5f, 0f, 1f);
        
        WalkTo(newPosition);
    }

    private void OnWalkLeft()
    {
        Vector3 newPosition = kaijuPivotTransform.position;
        newPosition += new Vector3(-.5f, 0f, 1f);
        
        WalkTo(newPosition);
    }

    private void WalkTo(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        Quaternion newRotation = Quaternion.LookRotation(direction, Vector3.up);

        targetPosition = transform.position + Vector3.forward * stepDistance;
        transform.DOMove(targetPosition, 0.25f);
        transform.DORotateQuaternion(newRotation, 0.25f);
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