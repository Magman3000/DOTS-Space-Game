using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics; 
using Unity.Physics;
using Unity.Burst;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
public partial class DamageSystem : SystemBase
{
    private GameData gameData;
	private bool active = false;
    protected override void OnCreate()
    {
        RequireForUpdate<Enemy>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
		if(!active)
		{
			gameData = SystemAPI.GetSingleton<GameData>();
			active = true;
		}
        foreach ((RefRO<LocalTransform> el, RefRW<CharacterDataComp> enemy) in SystemAPI
                     .Query<RefRO<LocalTransform>, RefRW<CharacterDataComp>>().WithAll<Enemy>())
        {
			foreach ((RefRO<LocalTransform> al, RefRW<AttackDataComp> attack) in SystemAPI
                     .Query<RefRO<LocalTransform>, RefRW<AttackDataComp>>())
			{
				float3 f = el.ValueRO.Position;
				float3 e = al.ValueRO.Position;
				if ((Mathf.Sqrt(Mathf.Pow((f.x - e.x), 2) + Mathf.Pow((f.y - e.y), 2))) < attack.ValueRO.damageRange )
        		{
            		enemy.ValueRW.HP -= attack.ValueRO.damage;
					enemy.ValueRW.hits++;
        		    attack.ValueRW.hit = true;
       			}
			}
		
        }
        foreach ((RefRO<LocalTransform> pl, RefRW<CharacterDataComp> player) in SystemAPI
                     .Query<RefRO<LocalTransform>, RefRW<CharacterDataComp>>().WithNone<Enemy>())
        {
			foreach ((RefRO<LocalTransform> el, RefRW<CharacterDataComp> enemy) in SystemAPI
                     .Query<RefRO<LocalTransform>, RefRW<CharacterDataComp>>().WithAll<Enemy>())
			{
				float3 f = pl.ValueRO.Position;
				float3 e = el.ValueRO.Position;
				if ((Mathf.Sqrt(Mathf.Pow((f.x - e.x), 2) + Mathf.Pow((f.y - e.y), 2))) < enemy.ValueRO.damageRange )
        		{
            		if(player.ValueRO.activeIFrames > 0) return;
            		player.ValueRW.HP -= enemy.ValueRO.damage;
            		enemy.ValueRW.HP = 0;
            		enemy.ValueRW.damage = 0;
            		player.ValueRW.activeIFrames = player.ValueRO.iFrame;
        		}
			}
            
            
            if (player.ValueRO.activeIFrames > 0)
            {
                float f = player.ValueRO.activeIFrames;
                f -= SystemAPI.Time.DeltaTime;
                if (f < 0) f = 0;
                player.ValueRW.activeIFrames = f;
            }
        }
        
        
    }
}
