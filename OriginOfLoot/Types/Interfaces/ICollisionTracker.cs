using System.Collections.Generic;

namespace OriginOfLoot.Types.Interfaces
{
    public interface ICollisionTracker
    {
        List<IEntity> HasCollidedWith { get; set; }
    }
}
