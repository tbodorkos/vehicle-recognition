using System.Threading.Tasks;

namespace Functions.ML
{
    public interface ITFModelScorer
    {
        Task<bool> Train();
        Task Test();
        Task<string> ClassifySingleImage(string imagePath);
    }
}
