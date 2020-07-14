using System;
using Unity.Entities;
using Unity.Physics;

[InternalBufferCapacity(8)]
[Serializable]
public struct PrefabListContentBuffer : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator Entity(PrefabListContentBuffer e) { return e.Content; }
    public static implicit operator PrefabListContentBuffer(Entity e) { return new PrefabListContentBuffer { Content = e }; }

    // Actual value each buffer element will store.
    public Entity Content;
}
