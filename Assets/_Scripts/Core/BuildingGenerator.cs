using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
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
        
        public void Initialize(BuildingGenerator generator, float levelHeight, int levels)
        {
            this.generator = generator;
            this.generator.OnRegenerate += Generate;
            
            this.levelHeight = levelHeight;
            this.levels = levels;

            transform.position = this.generator.transform.position;
            transform.rotation = this.generator.transform.rotation;
        }

        private void Generate()
        {
            Vector3 instantiationPosition = new Vector3(0, 0, 0);

            for (int i = transform.childCount; i > 0; i--)
            {
                DestroyImmediate(transform.GetChild(i)?.gameObject);
            }

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
    }
        
    public MeshRenderer[] FloorPrefabs;
    public MeshRenderer[] WallPrefabs;
    public MeshRenderer[] WindowPrefabs;
    public MeshRenderer[] RoofPrefabs;
    
    public List<Building> Buildings;
    
    public float LevelHeight;
    
    // Eight Stages
    public int[] LevelsPerStage = { 4, 6, 8, 10, 12, 14, 16, 18 };
    public int[] LevelVariancePerStage = { 2, 2, 3, 3, 4, 4, 5, 5 };

    [Button("Create Building")]
    public Building CreateBuilding()
    {
        var stageIndex = 0;
        var buildingGO = new GameObject("Building");
        var building = buildingGO.AddComponent<Building>();
        
        int randomNegative = Random.value < 0.5 ? -1 : 1;
        int levels = LevelsPerStage[stageIndex] + LevelVariancePerStage[stageIndex] * randomNegative;
        
        building.Initialize(this, LevelHeight, levels);
        Buildings.Add(building);
        
        OnRegenerate?.Invoke();
        return building;
    }

    public void Regenerate()
    {
        OnRegenerate?.Invoke();
    }
}
