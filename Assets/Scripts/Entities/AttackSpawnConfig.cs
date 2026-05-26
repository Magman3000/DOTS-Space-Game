using UnityEngine;
using Unity.Entities;

public class AttackSpawnConfig : MonoBehaviour
{
    public GameObject prefab;
    
    private class Baker : Baker<AttackSpawnConfig>
    {
        public override void Bake(AttackSpawnConfig authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new AttackConfig { prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic)});
        }
    }
}
public struct AttackConfig : IComponentData
{
    public Entity prefabEntity;
}