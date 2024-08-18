using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

public class ResourceGame : MonoBehaviour
{
    [FormerlySerializedAs("timer")] 
    public WorkHandler workHandler;

    public int Workers;
    public Resource Resource;

    public SpriteRenderer resourceIconRenderer;
    public SpriteMask resourceMask;
    
    public AssetReference stageSprite;
    public List<SpriteRenderer> stageRenderers = new();
    
    public List<Transform> stagesTransforms = new();
    
    public void Start()
    {
        workHandler.Init(new WorkHandler.WorkData(Resource.workCycle, Workers));
        workHandler.StartTimer();
        workHandler.OnTimerEnd += HandleTimerEnd;

        StartCoroutine(InstantiateStageSpriteRenderer(Resource.stages));

        resourceIconRenderer.sprite = Resource.resourceSprite;
        resourceMask.sprite = Resource.resourceSprite;
    }

    private IEnumerator InstantiateStageSpriteRenderer(int num)
    {
        List<SpriteRenderer> stageRenderers = new List<SpriteRenderer>(num);
        
        var asyncLoad = stageSprite.LoadAssetAsync<GameObject>();
            
        while (!asyncLoad.IsDone)
            yield return null;
        
        for (int i = 0; i < num; i++)
        {
            var stage = Instantiate(asyncLoad.Result, stagesTransforms[i].position, Quaternion.identity).GetComponent<SpriteRenderer>();
            stage.transform.parent = transform;
            
            stageRenderers.Add(stage);
            stage.color = Color.gray;
        }
        
        this.stageRenderers = stageRenderers;
    }

    private void Update()
    {
        workHandler.UpdateTimer(Time.deltaTime);
    }
    
    private int _currentStage = 0;
    private void HandleTimerEnd()
    {
        if (_currentStage > stageRenderers.Count - 1)
        {
            Debug.Log("Resource Gained");
            return;
        }

        Debug.Log("Stage Gained");
        stageRenderers[_currentStage].GetComponent<SpriteRenderer>().color = Color.white;
        _currentStage++;
        
        // Advance resource mask
        resourceMask.transform.GetChild(0).localPosition += Vector3.up * 1.5f / stageRenderers.Count;
    }
}

[Serializable]
public class WorkHandler
{
    [Serializable]
    public struct WorkData
    {
        public float workCycle;
        public int workers;

        public WorkData(float workCycle, int workers)
        {
            this.workCycle = workCycle;
            this.workers = workers;
        }
    }
    
    public SpriteRenderer spriteRenderer;
    public Sprite[] timerSprites;
    
    [ReadOnly, SerializeField]
    private float timer;
    
    private WorkData _workData;

    public float Progress => timer;
    
    public Action OnTimerStart;
    public Action OnTimerEnd;

    public void Init(WorkData workData)
    {
        _workData = workData;
    }
    public void StartTimer()
    {
        OnTimerStart?.Invoke();
    }

    public void UpdateTimer(float time)
    {
        timer += time * (1 / (_workData.workCycle / _workData.workers));
        spriteRenderer.sprite = timerSprites[ProgressToSpriteIndex(Progress)];

        if (timer < 1f) 
            return;
        
        OnTimerEnd?.Invoke();
        timer = 0f;
    }

    private int ProgressToSpriteIndex(float progress)
    {
        if (progress < 0.2)
            return 0;

        if (progress < 0.45)
            return 1;
        
        if (progress < 0.7)
            return 2;

        if (progress < 0.9f)
            return 3;

        return 4;
    }
}