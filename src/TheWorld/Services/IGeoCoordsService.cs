using System.Threading.Tasks;

namespace TheWorld.Services
{
    public interface IGeoCoordsService
    {
        Task<GeoCoordsResult> GetCoordsAsync(string name);
    }
}