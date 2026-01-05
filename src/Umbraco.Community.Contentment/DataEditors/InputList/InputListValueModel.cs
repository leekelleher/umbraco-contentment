namespace Umbraco.Community.Contentment.DataEditors;

internal sealed class InputListValueModel
{
    public Guid Alias { get; init; } = Guid.Empty;

    public object? Value { get; set; }
}

public class InputListTuples<T1> : List<Tuple<T1>> { }
public class InputListTuples<T1, T2> : List<Tuple<T1, T2>> { }
public class InputListTuples<T1, T2, T3> : List<Tuple<T1, T2, T3>> { }
public class InputListTuples<T1, T2, T3, T4> : List<Tuple<T1, T2, T3, T4>> { }
public class InputListTuples<T1, T2, T3, T4, T5> : List<Tuple<T1, T2, T3, T4, T5>> { }
public class InputListTuples<T1, T2, T3, T4, T5, T6> : List<Tuple<T1, T2, T3, T4, T5, T6>> { }
public class InputListTuples<T1, T2, T3, T4, T5, T6, T7> : List<Tuple<T1, T2, T3, T4, T5, T6, T7>> { }
public class InputListTuples<T1, T2, T3, T4, T5, T6, T7, TRest> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>> where TRest : notnull { }
