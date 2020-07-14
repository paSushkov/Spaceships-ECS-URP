using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(8)]
[Serializable]
public struct GridBufferContent : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator Entity(GridBufferContent e) { return e.Content; }
    public static implicit operator GridBufferContent(Entity e) { return new GridBufferContent { Content = e }; }

    // Actual value each buffer element will store.
    public Entity Content;
}
