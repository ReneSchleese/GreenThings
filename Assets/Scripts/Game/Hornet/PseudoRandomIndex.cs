using System.Linq;
using Random = UnityEngine.Random;

public class PseudoRandomIndex
{
    private readonly int[] _counters;
    private const int DEFAULT = 1;
    
    public PseudoRandomIndex(int length)
    {
        _counters = new int[length];
        for (var i = 0; i < _counters.Length; i++)
        {
            _counters[i] = DEFAULT;
        }
    }

    public int Get()
    {
        int index = DetermineIndex();
        IncrementAllBy(1);
        _counters[index] = DEFAULT;
        return index;
    }

    private int DetermineIndex()
    {
        int index = 0;
        int sum = _counters.Sum();
        int remainder = sum;
        int random = Random.Range(0, sum + 1);
        for (int i = _counters.Length - 1; i >= 0; i--)
        {
            bool IsInBucket() => random <= remainder && random > remainder - _counters[i];

            if (IsInBucket())
            {
                index = i;
                break;
            }

            remainder -= _counters[i];
        }
        
        return index;
    }

    private void IncrementAllBy(int value)
    {
        for (int i = 0; i < _counters.Length; i++)
        {
            _counters[i] += value;
        }
    }
}