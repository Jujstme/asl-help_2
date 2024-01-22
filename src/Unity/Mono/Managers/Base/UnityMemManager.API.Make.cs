using AslHelp.MemUtils.Exceptions;
using AslHelp.MemUtils.Pointers;
using AslHelp.Mono.Models;

namespace AslHelp.Mono.Managers;

public abstract partial class UnityMemManager
{
    public Pointer<T> Make<T>(string className, string staticFieldName, params object[] otherFields) where T : unmanaged
    {
        return Make<T>(className, 0, staticFieldName, otherFields);
    }

    public Pointer<T> Make<T>(string className, int parents, string staticFieldName, params object[] otherFields) where T : unmanaged
    {
        if (this is Il2CppManager)
        {
            string msg =
                "Inferring the class hierarchy automatically is currently not supported for IL2CPP games." + Environment.NewLine +
                "Please use the `Make` overloads from `MonoClass` instead.";

            throw new NotSupportedException(msg);
        }

        return Make<T>(this[className, parents], staticFieldName, otherFields);
    }

    public Pointer<T> Make<T>(MonoClass monoClass, string staticFieldName, params object[] otherFields) where T : unmanaged
    {
        if (this is Il2CppManager)
        {
            string msg =
                "Inferring the class hierarchy automatically is currently not supported for IL2CPP games." + Environment.NewLine +
                "Please use the `Make` overloads from `MonoClass` instead.";

            throw new NotSupportedException(msg);
        }

        if (monoClass.Static == 0)
        {
            throw new InvalidAddressException($"Address to static fields for class {monoClass.Name} was 0.");
        }

        (int staticFieldOffset, int[] offsets) = ResolveOffsets(monoClass, staticFieldName, otherFields);

        return new(monoClass.Static + staticFieldOffset, offsets);
    }

    public UnityStringPointer MakeString(string className, string staticFieldName, params object[] otherFields)
    {
        return MakeString(className, 0, staticFieldName, otherFields);
    }

    public UnityStringPointer MakeString(string className, int parents, string staticFieldName, params object[] otherFields)
    {
        if (this is Il2CppManager)
        {
            string msg =
                "Inferring the class hierarchy automatically is currently not supported for IL2CPP games." + Environment.NewLine +
                "Please use the `Make` overloads from `MonoClass` instead.";

            throw new NotSupportedException(msg);
        }

        return MakeString(this[className, parents], staticFieldName, otherFields);
    }

    public UnityStringPointer MakeString(MonoClass monoClass, string staticFieldName, params object[] otherFields)
    {
        if (this is Il2CppManager)
        {
            string msg =
                "Inferring the class hierarchy automatically is currently not supported for IL2CPP games." + Environment.NewLine +
                "Please use the `Make` overloads from `MonoClass` instead.";

            throw new NotSupportedException(msg);
        }

        if (monoClass.Static == 0)
        {
            throw new InvalidAddressException($"Address to static fields for class {monoClass.Name} was 0.");
        }

        (int staticFieldOffset, int[] offsets) = ResolveOffsets(monoClass, staticFieldName, otherFields);

        return new(monoClass.Static + staticFieldOffset, offsets);
    }

    public ArrayPointer<T> MakeArray<T>(string className, string staticFieldName, params object[] otherFields) where T : unmanaged
    {
        return MakeArray<T>(className, 0, staticFieldName, otherFields);
    }

    public ArrayPointer<T> MakeArray<T>(string className, int parents, string staticFieldName, params object[] otherFields) where T : unmanaged
    {
        if (this is Il2CppManager)
        {
            string msg =
                "Inferring the class hierarchy automatically is currently not supported for IL2CPP games." + Environment.NewLine +
                "Please use the `Make` overloads from `MonoClass` instead.";

            throw new NotSupportedException(msg);
        }

        return MakeArray<T>(this[className, parents], staticFieldName, otherFields);
    }

    public ArrayPointer<T> MakeArray<T>(MonoClass monoClass, string staticFieldName, params object[] otherFields) where T : unmanaged
    {
        if (this is Il2CppManager)
        {
            string msg =
                "Inferring the class hierarchy automatically is currently not supported for IL2CPP games." + Environment.NewLine +
                "Please use the `Make` overloads from `MonoClass` instead.";

            throw new NotSupportedException(msg);
        }

        if (monoClass.Static == 0)
        {
            throw new InvalidAddressException($"Address to static fields for class {monoClass.Name} was 0.");
        }

        (int staticFieldOffset, int[] offsets) = ResolveOffsets(monoClass, staticFieldName, otherFields);

        return new(monoClass.Static + staticFieldOffset, offsets);
    }

    public ListPointer<T> MakeList<T>(string className, string staticFieldName, params object[] otherFields) where T : unmanaged
    {
        return MakeList<T>(className, 0, staticFieldName, otherFields);
    }

    public ListPointer<T> MakeList<T>(string className, int parents, string staticFieldName, params object[] otherFields) where T : unmanaged
    {
        if (this is Il2CppManager)
        {
            string msg =
                "Inferring the class hierarchy automatically is currently not supported for IL2CPP games." + Environment.NewLine +
                "Please use the `Make` overloads from `MonoClass` instead.";

            throw new NotSupportedException(msg);
        }

        return MakeList<T>(this[className, parents], staticFieldName, otherFields);
    }

    public ListPointer<T> MakeList<T>(MonoClass monoClass, string staticFieldName, params object[] otherFields) where T : unmanaged
    {
        if (this is Il2CppManager)
        {
            string msg =
                "Inferring the class hierarchy automatically is currently not supported for IL2CPP games." + Environment.NewLine +
                "Please use the `Make` overloads from `MonoClass` instead.";

            throw new NotSupportedException(msg);
        }

        if (monoClass.Static == 0)
        {
            throw new InvalidAddressException($"Address to static fields for class {monoClass.Name} was 0.");
        }

        (int staticFieldOffset, int[] offsets) = ResolveOffsets(monoClass, staticFieldName, otherFields);

        return new(monoClass.Static + staticFieldOffset, offsets);
    }

    private (int, int[]) ResolveOffsets(MonoClass monoClass, string staticFieldName, params object[] otherFields)
    {
        if (!monoClass.TryGetValue(staticFieldName, out MonoField monoField))
        {
            string msg =
                $"Field '{staticFieldName}' was not present in '{monoClass.Name}'. " +
                $"Ensure correct spelling. Names are case sensitive.";

            throw new NotFoundException(msg);
        }

        int staticFieldOffset = monoField.Offset;
        List<int> offsets = new(otherFields.Length);

        foreach (object arg in otherFields)
        {
            if (arg is int offset)
            {
                offsets.Add(offset);
                continue;
            }

            if (arg is not string fieldName)
            {
                string msg = "Arguments for UnityManager.Make must consist only of strings or integers.";
                throw new ArgumentException(msg);
            }

            MonoClass klass = monoField.Type.Class;
            if (!klass.TryGetValue(fieldName, out monoField))
            {
                string msg =
                    $"Field '{fieldName}' could not be found. " +
                    $"Ensure correct spelling. Names are case sensitive.";

                throw new NotFoundException(msg);
            }

            if (ClassIsValueType(klass.Address))
            {
                offsets[^1] += monoField.Offset;
            }
            else
            {
                offsets.Add(monoField.Offset);
            }
        }

        return (staticFieldOffset, offsets.ToArray());
    }
}
