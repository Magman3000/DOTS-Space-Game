using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class GameDataConfig : MonoBehaviour
{
    public float2 min, max;
    private class Baker : Baker<GameDataConfig>
    {
        public override void Bake(GameDataConfig authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameData { min = authoring.min, max = authoring.max});
        }
    }
}
public struct GameData : IComponentData
{
    public float2 min, max;
    public int score;
}