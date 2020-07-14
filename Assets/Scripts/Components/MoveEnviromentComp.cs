using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct MoveEnviromentComp : IComponentData
{
    // TAG comp, to aknowledge what to move as environment
}
