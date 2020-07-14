using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class LineGridAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    private int width = 1;
    public int lengh = 1;
    private int height =1;

    public float widthCellSize;
    public float lenghCellSize;
    public float heightCellSize;

    [Range(0f, 1f)] public float widhtCellCenterOffset;
    [Range(0f, 1f)] public float lenghtCellCenterOffset;
    [Range(0f, 1f)] public float heightCellCenterOffset;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var grid = new GridComp
        {
            width = width,
            lenght = lengh,
            height = height,

            widthSize = widthCellSize,
            lenghtSize = lenghCellSize,
            heightSize = heightCellSize,

            widhtCellCenterOffset = widhtCellCenterOffset,
            lenghtCellCenterOffset = lenghtCellCenterOffset,
            heightCellCenterOffset = heightCellCenterOffset
        };

        dstManager.AddComponentData(entity, grid);
        dstManager.AddBuffer<GridBufferContent>(entity);
    }

}
