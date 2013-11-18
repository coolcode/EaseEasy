using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public static class XmlExtensions {

    public static string Value(this XElement x, string name, string defaultValue = null) {
        return x.Value(name, t => t);
    }

    public static TValue Value<TValue>(this XElement x, string name, Func<string, TValue> convert, TValue defaultValue = default(TValue)) {
        if (x.Element(name) == null) {
            return defaultValue;
        }

        var v = convert(x.Element(name).Value);

        return v;
    }
}
