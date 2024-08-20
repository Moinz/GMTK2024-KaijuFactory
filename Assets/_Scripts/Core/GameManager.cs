using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance => _instance;
    
    [SerializeField]
    private AssetReference kaijuAsset;

    [SerializeField]
    private AssetReference[] itemAssets;
    
    public KaijuController kaijuController;
    [FormerlySerializedAs("items")] public Item[] itemInstances;
    
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

        StartCoroutine(LoadItemsAsync());
    }

    private IEnumerator LoadItemsAsync()
    {
        itemInstances = new Item[itemAssets.Length];
        for (int i = 0; i < itemAssets.Length; i++)
        {
            AsyncOperationHandle<Item> asyncHandle = itemAssets[i].LoadAssetAsync<Item>();

            if (!asyncHandle.IsValid())
                continue;

            yield return asyncHandle;

            Debug.Log(asyncHandle.IsDone);
            while (!asyncHandle.IsDone)
                yield return null;
            
            itemInstances[i] = asyncHandle.Result;
        }
    }
}