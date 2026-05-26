using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;

public partial struct EnemyControllerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
	    state.RequireForUpdate<Enemy>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
	    foreach ((RefRO<LocalTransform> player, RefRO<CharacterDataComp> characterDataComp) in SystemAPI
		             .Query<RefRO<LocalTransform>, RefRO<CharacterDataComp>>().WithNone<Enemy>())
	    {
		    EnemyControlJob job = new EnemyControlJob { player = player.ValueRO};
		    job.ScheduleParallel();
	    }
	    
    }
    
    [BurstCompile]
    [WithAll(typeof(Enemy))]
    public partial struct EnemyControlJob : IJobEntity
    {
        public LocalTransform player;
        public void Execute(in LocalTransform localTransform, ref CharacterDataComp dataComp)
        {
			Vector3 d = player.Position - localTransform.Position;
			d = d.normalized;
			float a = Mathf.Atan2(d.y, d.x);
			LocalTransform lt = localTransform;
			a = a * 180 * Mathf.PI;//Corectly converting from Radians breaks it... wwwwwhhhhyyy?
			Quaternion g = Quaternion.Euler(0,0,a);
			int i = 0;
			int j = 0;
			bool active = true;
			while(active)
			{
				lt = lt.RotateZ(-1);
				i++;
				float ft = Quaternion.Angle(lt.Rotation, g);
				if(ft < 1) active = false;
			}
			lt = localTransform;
			active = true;
			while(active)
			{
				lt = lt.RotateZ(1);
				j++;
				float ft = Quaternion.Angle(lt.Rotation, g);
				if(ft < 1) active = false;
			}
			dataComp.i = i;
			dataComp.j = j;
			a = Quaternion.Angle(g, localTransform.Rotation);
			if (j > i) dataComp.rotation = -1.0f;
            else if (j < i) dataComp.rotation = 1.0f;
			else dataComp.rotation = 0;
			
        }
    }
}
