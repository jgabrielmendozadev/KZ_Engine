using System;
using System.Collections.Generic;

public class LookUpTable<T1, T2> {

    Dictionary<T1, T2> _values;
    Func<T1, T2> _operation;

    public void Reset() { _values = new Dictionary<T1, T2>(); }

    public LookUpTable(Func<T1, T2> op) {
        _operation = op;
        Reset();
    }

    public T2 Evaluate(T1 p) {
        if (!_values.ContainsKey(p)) _values[p] = _operation(p);
        return _values[p];
    }
}