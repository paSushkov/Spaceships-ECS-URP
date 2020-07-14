using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
[GenerateAuthoringComponent]
public class DestructionEffectComp : IComponentData
{
    public GameObject effectPrefab;
}
