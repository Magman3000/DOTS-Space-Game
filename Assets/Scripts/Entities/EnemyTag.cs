using UnityEngine;
using Unity.Entities;

public class EnemyTag : MonoBehaviour
{
    private class Baker : Baker<EnemyTag>
    {
        public override void Bake(EnemyTag authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Enemy());
        }
    }
}
public struct Enemy : IComponentData
{
    
}