
namespace EndlessHourglass.Gameplay.Interfaces
{
    public interface IDamageReceiver
    {
        int MaxHealth { get; }
        int CurrentHealth { get; set; }
    }
}
