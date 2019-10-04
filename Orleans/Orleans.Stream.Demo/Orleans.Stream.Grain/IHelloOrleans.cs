using System.Threading.Tasks;

namespace Orleans.Stream.Grain
{
    public interface IHelloOrleans:IGrainWithIntegerKey
    {
        Task<string> SayHi(string message);
    }
}