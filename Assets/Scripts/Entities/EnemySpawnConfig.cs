using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class EnemySpawnConfig : MonoBehaviour
{
    public GameObject prefab;
    public float2 positionMin, positionMax;
    
    private class Baker : Baker<EnemySpawnConfig>
    {
        public override void Bake(EnemySpawnConfig authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new EnemyConfig { prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic), positionMin = authoring.positionMin, positionMax = authoring.positionMax });
        }
    }
}
public struct EnemyConfig : IComponentData
{
    public Entity prefabEntity;
    public float2 positionMin, positionMax;
}
