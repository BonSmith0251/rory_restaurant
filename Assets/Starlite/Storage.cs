// flash.net.SharedObject emulation: a dynamic key/value object persisted as JSON in
// Application.persistentDataPath/<name>.json.  AS3 "so.data.x" becomes "so.data["x"]".
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Starlite
{
    public class SharedObject
    {
        static readonly Dictionary<string, SharedObject> sCache = new Dictionary<string, SharedObject>();
        public readonly string name;
        public Dictionary<string, object> data;

        SharedObject(string name)
        {
            this.name = name;
            data = new Dictionary<string, object>();
            try
            {
                if (File.Exists(FilePath) && Json.Parse(File.ReadAllText(FilePath)) is Dictionary<string, object> d) data = d;
            }
            catch (Exception e) { Debug.LogWarning("[SharedObject] could not read " + FilePath + ": " + e.Message); }
        }

        string FilePath => Path.Combine(Application.persistentDataPath, name + ".json");

        public static SharedObject getLocal(string name, string localPath = null, bool secure = false)
        {
            if (!sCache.TryGetValue(name, out var so)) sCache[name] = so = new SharedObject(name);
            return so;
        }

        public string flush(int minDiskSpace = 0)
        {
            try
            {
                File.WriteAllText(FilePath, Json.Serialize(data));
                return "flushed";
            }
            catch (Exception e) { Debug.LogWarning("[SharedObject] save failed: " + e.Message); return "pending"; }
        }

        public void clear() { data.Clear(); try { File.Delete(FilePath); } catch { } }

        /// <summary>data[key] or null when missing (AS3 returns undefined).</summary>
        public object get(string key) => data.TryGetValue(key, out var v) ? v : null;
        public bool has(string key) => data.ContainsKey(key) && data[key] != null;
    }

    /// <summary>Tiny JSON reader/writer (objects, arrays, numbers, strings, bools, null).</summary>
    public static class Json
    {
        public static object Parse(string s)
        {
            int i = 0;
            var v = Value(s, ref i);
            return v;
        }

        static void Ws(string s, ref int i) { while (i < s.Length && char.IsWhiteSpace(s[i])) i++; }

        static object Value(string s, ref int i)
        {
            Ws(s, ref i);
            if (i >= s.Length) return null;
            char c = s[i];
            if (c == '{')
            {
                var d = new Dictionary<string, object>();
                i++;
                Ws(s, ref i);
                if (s[i] == '}') { i++; return d; }
                while (true)
                {
                    Ws(s, ref i);
                    string k = Str(s, ref i);
                    Ws(s, ref i);
                    i++; // :
                    d[k] = Value(s, ref i);
                    Ws(s, ref i);
                    if (s[i] == ',') { i++; continue; }
                    i++; // }
                    return d;
                }
            }
            if (c == '[')
            {
                var l = new List<object>();
                i++;
                Ws(s, ref i);
                if (s[i] == ']') { i++; return l; }
                while (true)
                {
                    l.Add(Value(s, ref i));
                    Ws(s, ref i);
                    if (s[i] == ',') { i++; continue; }
                    i++; // ]
                    return l;
                }
            }
            if (c == '"') return Str(s, ref i);
            if (s.Substring(i).StartsWith("true")) { i += 4; return true; }
            if (s.Substring(i).StartsWith("false")) { i += 5; return false; }
            if (s.Substring(i).StartsWith("null")) { i += 4; return null; }
            int st = i;
            while (i < s.Length && "+-0123456789.eE".IndexOf(s[i]) >= 0) i++;
            string num = s.Substring(st, i - st);
            if (num.IndexOfAny(new[] { '.', 'e', 'E' }) < 0 && long.TryParse(num, NumberStyles.Integer, CultureInfo.InvariantCulture, out var lv))
                return lv >= int.MinValue && lv <= int.MaxValue ? (object)(int)lv : lv;
            return double.Parse(num, CultureInfo.InvariantCulture);
        }

        static string Str(string s, ref int i)
        {
            var sb = new StringBuilder();
            i++; // opening quote
            while (s[i] != '"')
            {
                char c = s[i++];
                if (c == '\\')
                {
                    char e = s[i++];
                    switch (e)
                    {
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'u': sb.Append((char)Convert.ToInt32(s.Substring(i, 4), 16)); i += 4; break;
                        default: sb.Append(e); break;
                    }
                }
                else sb.Append(c);
            }
            i++;
            return sb.ToString();
        }

        public static string Serialize(object o)
        {
            var sb = new StringBuilder();
            Write(sb, o);
            return sb.ToString();
        }

        static void Write(StringBuilder sb, object o)
        {
            switch (o)
            {
                case null: sb.Append("null"); break;
                case string s:
                    sb.Append('"');
                    foreach (char c in s)
                    {
                        if (c == '"' || c == '\\') sb.Append('\\').Append(c);
                        else if (c < 32) sb.Append("\\u").Append(((int)c).ToString("x4"));
                        else sb.Append(c);
                    }
                    sb.Append('"');
                    break;
                case bool b: sb.Append(b ? "true" : "false"); break;
                case float f: sb.Append(float.IsNaN(f) || float.IsInfinity(f) ? "0" : f.ToString("R", CultureInfo.InvariantCulture)); break;
                case double d: sb.Append(double.IsNaN(d) || double.IsInfinity(d) ? "0" : d.ToString("R", CultureInfo.InvariantCulture)); break;
                case IDictionary dict:
                    sb.Append('{');
                    bool first = true;
                    foreach (DictionaryEntry kv in dict)
                    {
                        if (!first) sb.Append(',');
                        first = false;
                        Write(sb, kv.Key.ToString());
                        sb.Append(':');
                        Write(sb, kv.Value);
                    }
                    sb.Append('}');
                    break;
                case IEnumerable list:
                    sb.Append('[');
                    bool f1 = true;
                    foreach (var item in list)
                    {
                        if (!f1) sb.Append(',');
                        f1 = false;
                        Write(sb, item);
                    }
                    sb.Append(']');
                    break;
                case IConvertible conv: sb.Append(Convert.ToString(conv, CultureInfo.InvariantCulture)); break;
                default: Write(sb, o.ToString()); break;
            }
        }
    }
}
