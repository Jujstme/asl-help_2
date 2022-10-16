namespace AslHelp.MemUtils.Pointers;

public class ArrayPointer<T> : BasePointer<T[]> where T : unmanaged
{
    private static readonly Unity _game = Unity.Instance;

    private T[] _buffer = Array.Empty<T>();

    public ArrayPointer(nint baseAddress, int[] offsets)
        : base(baseAddress, offsets) { }

    public override bool Write(T[] value)
    {
        throw new NotImplementedException();
    }

    protected override bool TryUpdate(out T[] result)
    {
        if (!_game.TryRead<nint>(out nint deref, _base, _offsets))
        {
            result = Array.Empty<T>();
            return false;
        }

        if (!_game.TryRead<int>(out int length, deref + (_game.PtrSize * 3)))
        {
            result = Array.Empty<T>();
            return false;
        }

        if (length != _buffer.Length)
        {
            _buffer = new T[length];
        }

        if (!_game.TryReadSpan_Internal<T>(_buffer, deref + (_game.PtrSize * 4)))
        {
            result = Array.Empty<T>();
            return false;
        }

        result = _buffer;

        return true;
    }

    protected override bool HasChanged(T[] old, T[] current)
    {
        return old.Length != current.Length || !old.SequenceEqual(current);
    }

    public override string ToString()
    {
        return $"ArrayPointer<{typeof(T).Name}>({OffsetsToString()})";
    }
}
