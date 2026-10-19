// ActionScript 3 runtime semantics that ported code relies on.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Starlite
{
    public static class AS3
    {
        static readonly System.Random sRng = new System.Random();
        static readonly System.Diagnostics.Stopwatch sClock = System.Diagnostics.Stopwatch.StartNew();

        /// <summary>Math.random(): [0, 1).</summary>
        public static float random() => (float)sRng.NextDouble();

        /// <summary>flash.utils.getTimer(): ms since start.</summary>
        public static int getTimer() => (int)sClock.ElapsedMilliseconds;

        public static void trace(params object[] args) => Debug.Log(string.Join(" ", args.Select(str)));

        /// <summary>AS3 Number(x).</summary>
        public static float toNumber(object o)
        {
            switch (o)
            {
                case null: return 0;
                case float f: return f;
                case double d: return (float)d;
                case int i: return i;
                case uint u: return u;
                case long l: return l;
                case bool b: return b ? 1 : 0;
                case XMLList xl: return toNumber(xl.ToString());
                case XML x: return toNumber(x.ToString());
                case string s:
                    s = s.Trim();
                    if (s.Length == 0) return 0;
                    if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) &&
                        long.TryParse(s.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hx)) return hx;
                    return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : float.NaN;
                default:
                    try { return Convert.ToSingle(o, CultureInfo.InvariantCulture); } catch { return float.NaN; }
            }
        }

        /// <summary>AS3 int(x): truncates, NaN/Infinity -> 0, wraps to 32 bits.</summary>
        public static int toInt(object o)
        {
            if (o is int i) return i;
            double d = o is double dd ? dd : toNumber(o);
            if (double.IsNaN(d) || double.IsInfinity(d)) return 0;
            return unchecked((int)(long)Math.Truncate(d));
        }

        public static uint toUint(object o) => unchecked((uint)toInt(o));

        /// <summary>AS3 Boolean(x).</summary>
        public static bool toBool(object o)
        {
            switch (o)
            {
                case null: return false;
                case bool b: return b;
                case string s: return s.Length > 0;
                case float f: return f != 0 && !float.IsNaN(f);
                case double d: return d != 0 && !double.IsNaN(d);
                case int i: return i != 0;
                case uint u: return u != 0;
                case long l: return l != 0;
                default: return true;
            }
        }

        /// <summary>AS3 String(x) / number formatting (no trailing ".0", invariant culture).</summary>
        public static string str(object o)
        {
            switch (o)
            {
                case null: return "null";
                case string s: return s;
                case bool b: return b ? "true" : "false";
                case float f when float.IsInfinity(f): return f > 0 ? "Infinity" : "-Infinity";
                case double d2 when double.IsInfinity(d2): return d2 > 0 ? "Infinity" : "-Infinity";
                case float f: return float.IsNaN(f) ? "NaN" : (f == Math.Floor(f) && Math.Abs(f) < 1e15 ? ((long)f).ToString(CultureInfo.InvariantCulture) : ((double)(decimal)f).ToString("R", CultureInfo.InvariantCulture));
                case double d: return double.IsNaN(d) ? "NaN" : (d == Math.Floor(d) && Math.Abs(d) < 1e15 ? ((long)d).ToString(CultureInfo.InvariantCulture) : d.ToString("R", CultureInfo.InvariantCulture));
                case IConvertible c: return Convert.ToString(c, CultureInfo.InvariantCulture);
                default: return o.ToString();
            }
        }

        /// <summary>Calls a callback with AS3-style loose arguments (missing args become defaults, extras are dropped).</summary>
        public static object Invoke(Delegate d, object[] args = null)
        {
            if (d == null) return null;
            if (d is Action a0) { a0(); return null; }
            var ps = d.Method.GetParameters();
            var call = new object[ps.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                object v = args != null && i < args.Length ? args[i] : (ps[i].HasDefaultValue ? ps[i].DefaultValue : null);
                call[i] = Coerce(v, ps[i].ParameterType);
            }
            try { return d.DynamicInvoke(call); }
            catch (TargetInvocationException e) { throw e.InnerException ?? e; }
        }

        static object Coerce(object v, Type t)
        {
            if (v == null) return t.IsValueType ? Activator.CreateInstance(t) : null;
            if (t.IsInstanceOfType(v)) return v;
            if (t == typeof(int)) return toInt(v);
            if (t == typeof(uint)) return toUint(v);
            if (t == typeof(float)) return toNumber(v);
            if (t == typeof(double)) return (double)toNumber(v);
            if (t == typeof(bool)) return toBool(v);
            if (t == typeof(string)) return str(v);
            return v;
        }

        public static bool isNaN(float f) => float.IsNaN(f);
        public static int parseInt(string s, int radix = 10)
        {
            s = (s ?? "").Trim();
            int end = 0;
            if (end < s.Length && (s[end] == '-' || s[end] == '+')) end++;
            while (end < s.Length && char.IsDigit(s[end])) end++;
            return int.TryParse(s.Substring(0, end), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : 0;
        }
        public static float parseFloat(string s)
        {
            s = (s ?? "").Trim();
            int end = 0;
            while (end < s.Length && "+-0123456789.eE".IndexOf(s[end]) >= 0) end++;
            return float.TryParse(s.Substring(0, end), NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : float.NaN;
        }

        /// <summary>Loads an asset that was embedded in the SWF ([Embed] class), e.g. "MainMenuScreen_KomikandyGold".</summary>
        public static Texture embeddedTexture(string symbol)
        {
            var t = Resources.Load<Texture2D>(AssetManager.ResourceRoot + "embedded/" + symbol);
            if (t == null) Debug.LogWarning("[AS3] embedded texture missing: " + symbol);
            return t != null ? Texture.fromUnity(t) : Texture.empty(1, 1);
        }

        public static XML embeddedXml(string symbol)
        {
            var t = Resources.Load<TextAsset>(AssetManager.ResourceRoot + "embedded/" + symbol);
            if (t == null) { Debug.LogWarning("[AS3] embedded xml missing: " + symbol); return new XML("<empty/>"); }
            return new XML(t.text);
        }
    }

    /// <summary>AS3 String / Array methods as extensions, so ported code can keep the original calls.</summary>
    public static class AS3Extensions
    {
        // ---------------- String
        public static string substr(this string s, int start, int len = int.MaxValue)
        {
            if (start < 0) start = Math.Max(0, s.Length + start);
            if (start >= s.Length || len <= 0) return "";
            return s.Substring(start, Math.Min(len, s.Length - start));
        }

        public static string substring(this string s, int start, int end = int.MaxValue)
        {
            start = Math.Max(0, Math.Min(start, s.Length));
            end = Math.Max(0, Math.Min(end, s.Length));
            if (start > end) { var t = start; start = end; end = t; }
            return s.Substring(start, end - start);
        }

        public static string slice(this string s, int start = 0, int end = int.MaxValue)
        {
            if (start < 0) start = Math.Max(0, s.Length + start);
            if (end < 0) end = Math.Max(0, s.Length + end);
            start = Math.Min(start, s.Length); end = Math.Min(end, s.Length);
            return end > start ? s.Substring(start, end - start) : "";
        }

        public static string charAt(this string s, int i) => i >= 0 && i < s.Length ? s[i].ToString() : "";
        public static int charCodeAt(this string s, int i) => i >= 0 && i < s.Length ? s[i] : 0;
        public static int indexOf(this string s, string v, int from = 0) => from >= s.Length ? (v.Length == 0 ? s.Length : -1) : s.IndexOf(v, Math.Max(0, from), StringComparison.Ordinal);
        public static int lastIndexOf(this string s, string v) => s.LastIndexOf(v, StringComparison.Ordinal);
        public static List<string> split(this string s, string delim) => delim == "" ? s.Select(c => c.ToString()).ToList() : s.Split(new[] { delim }, StringSplitOptions.None).ToList();
        public static string toUpperCase(this string s) => s.ToUpperInvariant();
        public static string toLowerCase(this string s) => s.ToLowerInvariant();
        /// <summary>AS3 String.replace with a string pattern replaces the FIRST occurrence only.</summary>
        public static string replace(this string s, string pattern, string repl)
        {
            int i = s.IndexOf(pattern, StringComparison.Ordinal);
            return i < 0 ? s : s.Substring(0, i) + repl + s.Substring(i + pattern.Length);
        }
        public static string replaceAll(this string s, string pattern, string repl) => s.Replace(pattern, repl);
        public static string concat(this string s, params object[] more) => s + string.Concat(more.Select(AS3.str));

        // ---------------- Array / Vector (List<T>)
        public static int push<T>(this List<T> l, params T[] items) { l.AddRange(items); return l.Count; }
        public static T pop<T>(this List<T> l) { if (l.Count == 0) return default; var v = l[l.Count - 1]; l.RemoveAt(l.Count - 1); return v; }
        public static T shift<T>(this List<T> l) { if (l.Count == 0) return default; var v = l[0]; l.RemoveAt(0); return v; }
        public static int unshift<T>(this List<T> l, params T[] items) { l.InsertRange(0, items); return l.Count; }

        public static List<T> splice<T>(this List<T> l, int start, int deleteCount = int.MaxValue, params T[] items)
        {
            if (start < 0) start = Math.Max(0, l.Count + start);
            start = Math.Min(start, l.Count);
            deleteCount = Math.Max(0, Math.Min(deleteCount, l.Count - start));
            var removed = l.GetRange(start, deleteCount);
            l.RemoveRange(start, deleteCount);
            l.InsertRange(start, items);
            return removed;
        }

        public static List<T> slice<T>(this List<T> l, int start = 0, int end = int.MaxValue)
        {
            if (start < 0) start = Math.Max(0, l.Count + start);
            if (end < 0) end = Math.Max(0, l.Count + end);
            start = Math.Min(start, l.Count); end = Math.Min(end, l.Count);
            return end > start ? l.GetRange(start, end - start) : new List<T>();
        }

        public static List<T> concat<T>(this List<T> l, params IEnumerable<T>[] more)
        {
            var r = new List<T>(l);
            foreach (var m in more) r.AddRange(m);
            return r;
        }

        public static string join<T>(this IEnumerable<T> l, string sep = ",") => string.Join(sep, l.Select(x => AS3.str(x)));
        public static int indexOf<T>(this List<T> l, T item, int from = 0) => from >= l.Count ? -1 : l.IndexOf(item, Math.Max(0, from));
        public static void reverse<T>(this List<T> l) => l.Reverse();

        /// <summary>Sets length like "vector.length = n" (truncates or pads with defaults).</summary>
        public static void setLength<T>(this List<T> l, int n)
        {
            if (n < l.Count) l.RemoveRange(n, l.Count - n);
            while (l.Count < n) l.Add(default);
        }

        /// <summary>Index access that grows the list like AS3 arrays do on assignment.</summary>
        public static void set<T>(this List<T> l, int i, T v)
        {
            while (l.Count <= i) l.Add(default);
            l[i] = v;
        }

        /// <summary>Read with AS3 out-of-range semantics (returns default instead of throwing).</summary>
        public static T at<T>(this List<T> l, int i) => i >= 0 && i < l.Count ? l[i] : default;

        public static List<T> filled<T>(int n, Func<int, T> make)
        {
            var l = new List<T>(n);
            for (int i = 0; i < n; i++) l.Add(make(i));
            return l;
        }
    }
}
