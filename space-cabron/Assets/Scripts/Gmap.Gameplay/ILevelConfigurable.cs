
namespace Gmap.Gameplay
{
    public interface ILevelConfigurable<LevelConfigurationType>
    {
        void Configure(LevelConfigurationType configuration);
    }
}