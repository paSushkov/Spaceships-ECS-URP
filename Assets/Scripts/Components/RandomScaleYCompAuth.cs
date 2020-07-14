using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class RandomScaleYCompAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [Tooltip("How BIG it can be")]
    [Range (1f, 15f)]
    public float MaxScale;
    [Tooltip("How SMALL it can be")]
    [Range (0f, 1f)]
    public float MinScale;
    [Tooltip("How QUICK should it change")]
    [Range (0f, 15f)]
    public float LerpSpeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var newRandomScaleComp = new RandomScaleYComp
        {
            MaxScaleY = MaxScale,
            MinScaleY = MinScale,
            LerpSpeed = this.LerpSpeed,
            DesiredYScale = gameObject.transform.localScale.y
        };
        dstManager.AddComponentData(entity, newRandomScaleComp);


    }
}
