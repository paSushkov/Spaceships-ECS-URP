using System.Collections.Generic;
using Unity.Entities;

public interface IPlayerStatsReciever
{
    void GetPlayerStats (PlayerHealthComp PlayerHealthComp, PlayerShieldComp PlayerShieldComp);
}
