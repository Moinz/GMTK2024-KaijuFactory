using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance => _instance;
    
    [SerializeField]
    private AssetReference kaijuAsset;
    public KaijuController kaijuController;
    
    public bool isInitialized = false;

    private void Awake()
    {
        _instance = this;

        if (kaijuController != null)
        {
            isInitialized = true;
            return;
        }
        
        kaijuAsset.LoadAssetAsync<KaijuController>().Completed += handle =>
        {
            isInitialized = true;
            kaijuController = handle.Result;
        };
    }
}