using System.Collections.Generic;

namespace EndlessHourglass.Types.Interfaces
{
    public interface ICollisionTracker
    {
        List<IEntity> HasCollidedWith { get; set; }
    }
}
