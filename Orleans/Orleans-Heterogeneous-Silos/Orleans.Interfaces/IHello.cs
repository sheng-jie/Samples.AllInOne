using System.Threading.Tasks;

namespace Orleans.Interfaces
{
    /// <summary>
    /// Orleans grain communication interface IHello
    /// </summary>
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
        Task<User> GetUser(int id);
    }
}
