using System;
using Unity.Entities;
using UnityEngine;

public class PlayerStatAuthoring : MonoBehaviour
{
    public PlayerStat[] playerStats = Array.Empty<PlayerStat>();

    public class PlayerStatBaker : Baker<PlayerStatAuthoring>
    {
        public override void Bake(PlayerStatAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var dynamicBuffer = AddBuffer<PlayerStat>(entity);
            foreach (var stat in authoring.playerStats)
            {
                dynamicBuffer.Add(new()
                {
                    Health = stat.Health, Mana = stat.Mana, Damage = stat.Damage
                });
            }
        }
    }
}