using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using InputActions;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

public partial class PlayerController : SystemBase
{
	public InputSystem_Actions inputActions;
	public bool active = false;
	public event EventHandler Death;
	public float health = 0;
    private AttackConfig config;
	private float cooldown;
    protected override void OnCreate()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
    }

    protected override void OnUpdate()
    {
        if (!active)
        {
            config = SystemAPI.GetSingleton<AttackConfig>();
            active = true;
        }
		foreach ((RefRW<CharacterDataComp> player, Entity entity) in SystemAPI.Query<RefRW<CharacterDataComp>>().WithNone<Enemy>().WithEntityAccess())
        {
			if(player.ValueRO.HP <= 0)
			{ 
				Death?.Invoke(entity, EventArgs.Empty);
				SystemAPI.SetComponent(entity, new LocalTransform{
					Position = new Vector3(0,0,0),
                	Rotation = Quaternion.identity,
                	Scale = 1.0f});
				player.ValueRW.HP = player.ValueRO.maxHP;
				cooldown = 0;
				player.ValueRW.activeIFrames = 0;
				return;
			}
			else health = player.ValueRO.HP;
		}
        float fM = inputActions.Player.Move.ReadValue<float>();
        float fR = inputActions.Player.Rotate.ReadValue<float>();
        bool b = (0 < inputActions.Player.Attack.ReadValue<float>());
        if (b && cooldown <= 0) Attack();
		else cooldown -= SystemAPI.Time.DeltaTime;
        PlayerMovementJob job = new PlayerMovementJob { move = fM, rotate = fR };
        job.ScheduleParallel();
    }

    protected override void OnDestroy()
    {
        inputActions.Player.Disable();
    }
    private void Attack()
    {
        foreach ((RefRO<LocalTransform> player, RefRO<CharacterDataComp> data) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<CharacterDataComp>>().WithNone<Enemy>())
        {
			cooldown = data.ValueRO.cooldown;
			LocalTransform localTransform = new LocalTransform
            {
                Position = player.ValueRO.Position,
                Rotation = player.ValueRO.Rotation,
                Scale = 0.4f
            };
			localTransform = localTransform.RotateZ(UnityEngine.Random.Range(-0.1f,0.1f));
            Entity spawned = EntityManager.Instantiate(config.prefabEntity);
            SystemAPI.SetComponent(spawned, localTransform);
        }
        
    }
	public void Reset(object sender)
	{
		foreach ((RefRW<CharacterDataComp> player, Entity entity) in SystemAPI.Query<RefRW<CharacterDataComp>>().WithNone<Enemy>().WithEntityAccess())
        {
			Death?.Invoke(sender, EventArgs.Empty);
			SystemAPI.SetComponent(entity, new LocalTransform{
				Position = new Vector3(0,0,0),
                Rotation = new Quaternion(0,0,0,0),
                Scale = 1.0f});
			player.ValueRW.HP = player.ValueRO.maxHP;
			cooldown = 0;
			player.ValueRW.activeIFrames = 0;
			return;
		}
	}
    
}

[BurstCompile]
[WithNone(typeof(Enemy))]
public partial struct PlayerMovementJob : IJobEntity
{
    public float move;
    public float rotate;
    public void Execute(ref CharacterDataComp dataComp)
    {
        dataComp.momentum = move;
        dataComp.rotation = rotate;
    }
}
