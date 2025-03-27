using System.Collections.Generic;

namespace EndlessHourglass.Gameplay.Interfaces
{
    public interface ICollisionTracker
    {
        List<IEntity> HasCollidedWith { get; set; }
    }
}
