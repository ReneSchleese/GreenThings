using System.Collections.Generic;

public abstract class AppStateParams { }

public class GameTransitionParams : AppStateParams
{
    public readonly List<VinylId> VinylIds;

    public GameTransitionParams(List<VinylId> vinylIds)
    {
        VinylIds = vinylIds;
    }
}