using UnityEngine;
using Unity.Entities;

public class AttackData : MonoBehaviour
{
    public float speed, damage, damageRange, lifetime;

    private class Baker : Baker<AttackData>
    {
        public override void Bake(AttackData authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AttackDataComp { speed = authoring.speed, damage = authoring.damage, damageRange = authoring.damageRange, hit = false, lifetime = authoring.lifetime, duration = 0, active = false});
        }
    }
}
public struct AttackDataComp : IComponentData
{
    public float speed, damage, damageRange, lifetime, duration;
    public bool hit;
	public bool active;
}
