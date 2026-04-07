namespace DeveMazeGeneratorCore.Mazes;

public interface IBacktrackStack<T>
{
    int Count { get; }

    void Push(T item);
    T Peek();
    void Pop();
}