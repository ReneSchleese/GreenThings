public class CircularBuffer<T>
{
    public CircularBuffer(int size)
    {
        Values = new T[size];
        Index = 0;
    }

    public void Add(T element)
    {
        Values[Index] = element;
        Index = (Index + 1) % Values.Length;
    }

    public T GetPreviousNth(int n)
    {
        var index = Utils.Mod(Index - n, Values.Length);
        return Values[index];
    }

    public T GetYoungest()
    {
        return GetPreviousNth(1);
    }

    public T GetOldest()
    {
        return Values[Index];
    }

    public void SetAll(T value)
    {
        for (var i = 0; i < Values.Length; i++)
        {
            Values[i] = value;
        }
    }

    public T Get(int index) => Values[index];
    public T[] Values { get; }
    public int Length => Values.Length;
    public int Index { get; private set; }
}