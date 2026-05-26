using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public partial class MovementSystem : SystemBase
{
    private GameData gameData;
    private bool active = false;
    protected override void OnCreate()
    {
        RequireForUpdate<CharacterDataComp>();
    }
    [BurstCompile]
    protected override void OnUpdate()
    {
        if(!active) gameData = SystemAPI.GetSingleton<GameData>();
        CharacterMovementJob charMoveJob = new CharacterMovementJob { ΔTime = SystemAPI.Time.DeltaTime, gameData =  gameData };
        AttackMovementJob attackMoveJob = new AttackMovementJob { ΔTime = SystemAPI.Time.DeltaTime, gameData =  gameData };
        charMoveJob.ScheduleParallel();
        attackMoveJob.ScheduleParallel();
    }
}
[BurstCompile]
public partial struct CharacterMovementJob : IJobEntity
{
    public float ΔTime;
    public GameData gameData;

    public void Execute(ref LocalTransform localTransform, ref CharacterDataComp dataComp)
    {
		dataComp.active = true;
        LocalTransform temp = localTransform.RotateZ(dataComp.rotation * ΔTime * dataComp.rotateSpeed);
        float3 f3 = localTransform.Up();
        temp = temp.Translate(f3 * dataComp.momentum * ΔTime * dataComp.moveSpeed);
        if (temp.Position.x < gameData.min.x) temp.Position.x = gameData.min.x;
        else if (temp.Position.x > gameData.max.x) temp.Position.x = gameData.max.x;
        if (temp.Position.y < gameData.min.y) temp.Position.y = gameData.min.y;
        else if (temp.Position.y > gameData.max.y) temp.Position.y = gameData.max.y;
        localTransform = temp;
    }
}
[BurstCompile]
public partial struct AttackMovementJob : IJobEntity
{
    public float ΔTime;
    public GameData gameData;
    public void Execute(ref LocalTransform localTransform, ref AttackDataComp dataComp)
    {
		dataComp.active = true;
		dataComp.duration += ΔTime;
        if (localTransform.Position.x < gameData.min.x - 2.0f || localTransform.Position.x > gameData.max.x + 2.0f || localTransform.Position.y < gameData.min.y - 2.0f || localTransform.Position.y > gameData.max.y + 2.0f) return;
        float3 f3 = localTransform.Up();
        quaternion q = localTransform.Rotation;
        localTransform = localTransform.Translate(f3 * ΔTime * dataComp.speed);
    }
}