using WowNavBase;

namespace WowNavApi.Services
{
    public interface INavigationService
    {
        Position[] CalculatePath(uint mapId, Position start, Position end, bool straightPath);
    }
}