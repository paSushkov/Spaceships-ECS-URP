using Unity.Entities;
using UnityEngine;

public class PlayerInputSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        EntityManager.CreateEntity(typeof(PlayerInput));
        SetSingleton(new PlayerInput());
    }

    protected override void OnUpdate()
    {
        var playerInput = GetSingleton<PlayerInput>();

        playerInput.Horizontal = Input.GetAxis("Horizontal");
        playerInput.Vertical = Input.GetAxis("Vertical");
        playerInput.Shoot = Input.GetMouseButton(0);
        
        SetSingleton(playerInput);
    }
}