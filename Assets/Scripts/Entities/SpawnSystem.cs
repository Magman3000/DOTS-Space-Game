using System;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

[BurstCompile]
public partial class SpawnSystem : SystemBase
{
    private EnemyConfig spawnConfig;
    private bool active, eCBRotation = false;
    private int index = 0;
	private EntityCommandBuffer eCB;
    private int[] waves = new int[7] { 5, 7, 9, 11, 13, 15, 17 };
	public int score = -1;
    
    protected override void OnCreate()
    {
        RequireForUpdate<EnemyConfig>();
		World.GetOrCreateSystemManaged<PlayerController>().Death += Player_Death;
    }

    protected override void OnUpdate()
    {
		if(eCBRotation) 
		{
			eCB.Playback(EntityManager);
			eCB.Dispose();
			eCBRotation = false;
			return;
		}
        if (!active)
        {
            spawnConfig = SystemAPI.GetSingleton<EnemyConfig>();
            active = true;
            Startup();
			return;
        }
		if (score == -1)
		{
			foreach (RefRW<GameData> gD in SystemAPI.Query<RefRW<GameData>>())
			{
				gD.ValueRW.score = 0;
			}
		}
		int next = 0;
		foreach(Entity e in EntityManager.GetAllEntities())
		{
			if(SystemAPI.HasComponent<Enemy>(e)) 
			{
				next++;
				CharacterDataComp dC = SystemAPI.GetComponent<CharacterDataComp>(e);
				if(dC.HP <= 0) 
				{
					if (dC.hits >= dC.maxHP)
					{
						ScoreJob job = new ScoreJob{reset = false};
						job.ScheduleParallel();
					}
					EntityManager.DestroyEntity(e);
				}
			} 
			else if(SystemAPI.HasComponent<AttackDataComp>(e)) 
			{
				AttackDataComp adc = SystemAPI.GetComponent<AttackDataComp>(e);
				if(adc.hit || adc.duration > adc.lifetime) EntityManager.DestroyEntity(e);
			}
		}
		score = SystemAPI.GetSingleton<GameData>().score;
		if(next == 1) Spawn(waves[index]);
    }

    public void Startup()
    {
        Spawn(waves[index]);
    }
    [BurstCompile]
    public void Spawn(int quantity)
    {
        if (!active) return;
		EntityCommandBuffer eCB = new EntityCommandBuffer(Allocator.TempJob);
        for (int i = 0; i < quantity; i++)
        {
            Entity spawned = eCB.Instantiate(spawnConfig.prefabEntity);
            eCB.SetComponent(spawned, new LocalTransform
            {
                Position = new float3(UnityEngine.Random.Range(spawnConfig.positionMin.x, spawnConfig.positionMax.x),
                    UnityEngine.Random.Range(spawnConfig.positionMin.y, spawnConfig.positionMax.y), 0.0f),
                Rotation = quaternion.identity,
                Scale = 1.0f
            });
        }
		eCB.Playback(EntityManager);
		eCB.Dispose();
        if (index < waves.Length -1) index++;
    }
	public void Player_Death(object sender, System.EventArgs a)
	{
		eCB = new EntityCommandBuffer(Allocator.TempJob);
		foreach(Entity e in EntityManager.GetAllEntities())
		{
			if(SystemAPI.HasComponent<Enemy>(e))
			{ 
				CharacterDataComp dC = SystemAPI.GetComponent<CharacterDataComp>(e);
				if(dC.active) eCB.DestroyEntity(e);
			} 
			else if(SystemAPI.HasComponent<AttackDataComp>(e)) 
			{
				AttackDataComp dC = SystemAPI.GetComponent<AttackDataComp>(e);
				if(dC.active) eCB.DestroyEntity(e);
			}
		}
		eCBRotation = true;
		active = false;
		index = 0;
	}
}
[BurstCompile]
public partial struct ScoreJob : IJobEntity
{
    public bool reset;
    public void Execute(ref GameData game)
    {
		if(reset) game.score = 0;
        else game.score++;
    }
}

