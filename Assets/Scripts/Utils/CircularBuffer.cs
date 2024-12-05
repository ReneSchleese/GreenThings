public class CircularBuffer<T>
{
    private readonly T[] _buffer;
    private int _index;
    
    public CircularBuffer(int size)
    {
        _buffer = new T[size];
        _index = 0;
    }

    public void Add(T element)
    {
        _buffer[_index] = element;
        _index = (_index + 1) % _buffer.Length;
    }

    public T GetLastNth(int n)
    {
        var index = Utils.Mod(_index - n, _buffer.Length);
        return _buffer[index];
    }

    public void SetAll(T value)
    {
        for (var i = 0; i < _buffer.Length; i++)
        {
            _buffer[i] = value;
        }
    }
}