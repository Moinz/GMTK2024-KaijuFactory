using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class BuildingGenerator : MonoBehaviour
{
    // API
    public Action OnRegenerate;
    
    public class Building : MonoBehaviour
    {
        public BuildingGenerator generator;
        public int levels;
        public float levelHeight;
        
        private float _currentLevelHealth;
        private float _levelHealth;
        
        public void Initialize(BuildingGenerator generator, float levelHeight, int levels)
        {
            this.generator = generator;
            this.generator.OnRegenerate += Generate;
            
            this.levelHeight = levelHeight;
            this.levels = levels;

            transform.position = this.generator.transform.position;
            transform.rotation = this.generator.transform.rotation;
        }

        /// <summary>
        /// Returns true if the damage resulted in an ejected level
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        public bool InflictDamage(float damage)
        {
            _currentLevelHealth -= damage;

            if (_currentLevelHealth > 0)
                return false;

            _currentLevelHealth += _levelHealth;
            EjectLevel();

            return transform.childCount == 0;
        }
        
        private void EjectLevel()
        {
            if (transform.childCount == 0)
                return;

            var child = transform.GetChild(0);

            var ejectPosition = transform.position + transform.right * 3f;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(child.DOJump(ejectPosition, 0.25f, 3, 0.5f).SetEase(Ease.InOutBounce));
            sequence.AppendCallback(() =>
            {
                Destroy(child.gameObject);
            });
            
            transform.localPosition += Vector3.down * levelHeight;
        }

        private void Start()
        {
            var rb = GetComponent<Rigidbody>();
            
            if (!rb)
                rb = gameObject.AddComponent<Rigidbody>();
            
            rb.isKinematic = true;
            gameObject.tag = "Building";
        }
        
        private void OnDestroy()
        {
            generator.OnRegenerate -= Generate;
        }

        private void Generate()
        {
            Vector3 instantiationPosition = new Vector3(0, 0, 0);
            Clear();

            MeshRenderer prefab = generator.WallPrefabs[Random.Range(0, generator.WallPrefabs.Length)];
            
            for (int i = 0; i < levels; i++)
            {
                if (i == 0)
                {
                    prefab = generator.FloorPrefabs[Random.Range(0, generator.FloorPrefabs.Length)];
                }
                else if (i + 1 == levels)
                {
                    prefab = generator.RoofPrefabs[Random.Range(0, generator.RoofPrefabs.Length)];
                }
                else if (Random.value < 0.25f)
                {
                    prefab = generator.WindowPrefabs[Random.Range(0, generator.WindowPrefabs.Length)];
                }
                
                Instantiate(prefab, transform.TransformPoint(instantiationPosition), Quaternion.identity, transform);
                instantiationPosition.y += levelHeight;
            }
        }

        internal void Clear(bool destroy = false)
        {
            if (!this)
                return;
            
            if (transform.childCount == 0)
                return;
            
            for (int i = transform.childCount - 1; i > 0; i--)
            {
                DestroyImmediate(transform.GetChild(i)?.gameObject);
            }

            if (!destroy) 
                return;
            
            OnDestroy();
            DestroyImmediate(gameObject);
        }
    }
        
    public MeshRenderer[] FloorPrefabs;
    public MeshRenderer[] WallPrefabs;
    public MeshRenderer[] WindowPrefabs;
    public MeshRenderer[] RoofPrefabs;
    
    public List<Building> Buildings;
    
    public float LevelHeight;
    public float BuildingDistance;
    
    // Eight Stages
    public int[] LevelsPerStage = { 4, 6, 8, 10, 12, 14, 16, 18 };
    public int[] LevelVariancePerStage = { 2, 2, 3, 3, 4, 4, 5, 5 };

    [Button("Create Building")]
    public Building CreateBuilding()
    {
        var stageIndex = 0;
        var buildingGO = new GameObject("Building");
        var building = buildingGO.AddComponent<Building>();
        
        transform.localPosition = Vector3.forward * (BuildingDistance * Buildings.Count);
        
        int randomNegative = Random.value < 0.5 ? -1 : 1;
        int levels = LevelsPerStage[stageIndex] + LevelVariancePerStage[stageIndex] * randomNegative;
        
        building.Initialize(this, LevelHeight, levels);
        Buildings.Add(building);
        
        OnRegenerate?.Invoke();
        return building;
    }

    [Button("Clear Buildings")]
    public void ClearBuildings()
    {
        for (int i = Buildings.Count - 1; i >= 0; i--)
        {
            Buildings[i].Clear(true);
            Buildings[i] = null;
        }
        
        Buildings.Clear();
    }

    [Button("Regenerate Buildings")]
    public void Regenerate()
    {
        OnRegenerate?.Invoke();
    }
}
