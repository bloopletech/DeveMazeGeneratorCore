namespace DeveMazeGeneratorCore.Mazes;

public class ContiguousBacktrackStack<T>(int capacity) : IBacktrackStack<T>
{
    private readonly Stack<T> stack = new(capacity);

    public ContiguousBacktrackStack() : this(0)
    {
    }

    public int Count => stack.Count;
    public void Push(T item) => stack.Push(item);
    public void Pop() => stack.Pop();
    public T Peek() => stack.Peek();
}
