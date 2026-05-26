using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class CharacterData : MonoBehaviour
{
    public float moveSpeed, rotateSpeed, maxHP, damage, momentum, iFrame, damageRange, cooldown;
    public float HP => maxHP;

    private class Baker : Baker<CharacterData>
    {
        public override void Bake(CharacterData authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CharacterDataComp { moveSpeed = authoring.moveSpeed, rotateSpeed = authoring.rotateSpeed,  maxHP = authoring.maxHP, HP = authoring.HP, damage = authoring.damage, momentum = authoring.momentum, rotation = 0, iFrame = authoring.iFrame, activeIFrames = 0, damageRange = authoring.damageRange, hits = 0, cooldown = authoring.cooldown, active = false });
        }
    }
}
public struct CharacterDataComp : IComponentData
{
    public float moveSpeed, rotateSpeed, maxHP, damage, HP, momentum, rotation, iFrame, activeIFrames, damageRange, cooldown;
	public float4 q;
	public int i, j, hits;
	public bool active;
}
