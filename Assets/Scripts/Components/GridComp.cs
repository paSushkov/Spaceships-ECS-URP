using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct GridComp : IComponentData
{
    public int width;
    public int lenght;
    public int height;

    public float widthSize;
    public float lenghtSize;
    public float heightSize;

    public float widhtCellCenterOffset;
    public float lenghtCellCenterOffset;
    public float heightCellCenterOffset;
}
