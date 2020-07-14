using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class UpdateListComp : IComponentData
{
    public List<IPlayerStatsReciever> recievers;
}
