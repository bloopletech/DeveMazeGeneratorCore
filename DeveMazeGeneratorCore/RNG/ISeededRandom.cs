namespace DeveMazeGeneratorCore.RNG;

public interface ISeededRandom<T> : IRandom
{
    T Seed { get; }

    ///// <summary>
    ///// Reinitialises the random generator with a new seed
    ///// </summary>
    ///// <param name="seed">The seed to use</param>
    //void Reinitialise(int seed);
}
