using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Tag-component, to "mark" entity for "NotifyMonobehavioursSystems" system.
/// </summary>
[Serializable]
public struct UpdateListNowTag : IComponentData
{
}
