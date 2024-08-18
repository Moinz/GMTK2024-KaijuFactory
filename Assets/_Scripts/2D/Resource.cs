using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Kaiju/Resource", order = 0)]
public class Resource : ScriptableObject
{
    public float workCycle;
    public int stages = 6;
    
    public Sprite resourceSprite;
}