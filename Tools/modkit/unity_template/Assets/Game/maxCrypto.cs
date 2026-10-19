// Ported from rudi.maxCrypto: digit<->letter obfuscation of numbers ("STI"/"ITS") and path helpers.
using System;
using System.Collections.Generic;
using Starlite;
using Mathf = UnityEngine.Mathf;

public class maxCrypto
{
    static maxCrypto _instance;
    public List<string> key = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
    public string separator = ",";

    maxCrypto() { }

    public static maxCrypto getInstance() => _instance ?? (_instance = new maxCrypto());

    public void loadKey(List<object> arg1)
    {
        for (int i = 0; i < 10; i++) key[i] = AS3.str(arg1[i]);
    }

    public void changeKey(int arg1, string arg2)
    {
        if (arg1 < 10 && arg1 > -1) key[arg1] = arg2;
    }

    /// <summary>Obfuscated string -> int (each char is looked up in the key; unknown chars are skipped).</summary>
    public int STI(string arg1)
    {
        int result = 0;
        if (arg1 == null) return 0;
        for (int i = 0; i < arg1.Length; i++)
        {
            string ch = arg1.charAt(i);
            for (int k = 0; k < 10; k++)
            {
                if (ch == key[k])
                {
                    result = result * 10;
                    result = result + k;
                    break;
                }
            }
        }
        return result;
    }

    /// <summary>int -> obfuscated string ("a" for 0).</summary>
    public string ITS(int arg1)
    {
        string s = "";
        if (arg1 == 0) return "a";
        while (arg1 > 0)
        {
            s = s + key[arg1 % 10];
            arg1 = arg1 / 10; // int(arg1 / 10) - positive, so truncation equals floor
        }
        var chars = s.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    public string convertToDigit(string arg1, int arg2, int arg3)
    {
        string str = null;
        if (arg3 == 1) str = arg1;
        if (arg3 == 2) str = "" + arg2;
        int len = str.Length;
        int groups = (int)Mathf.Floor(len / 3f);
        string result = "";
        int rest = len - groups * 3;
        if (rest > 0 || len > 1) groups++;
        for (int i = 0; i < groups; i++)
        {
            if ((rest > 0 || len > 1) && i == 0)
                result = "" + str.slice(0, rest);
            else if (result == "")
                result = result + str.slice(rest + i + (i - 1) * 2 - 1, rest + i + (i - 1) * 2 + 3 - 1);
            else
                result = result + separator + str.slice(rest + i + (i - 1) * 2 - 1, rest + i + (i - 1) * 2 + 3 - 1);
        }
        return result;
    }

    public int RandomRange(int arg1, int arg2) => (int)Mathf.Floor(AS3.random() * (1 + arg2 - arg1)) + arg1;

    public string toMinute(int arg1)
    {
        int rest = arg1;
        int minutes = 0, seconds = 0;
        if (rest >= 60) { minutes = (int)Mathf.Floor(rest / 60f); rest -= minutes * 60; }
        if (rest >= 1) { seconds = rest; rest -= seconds; }
        string m = "" + minutes, s = "" + seconds;
        if (m.Length == 1) m = "0" + m;
        if (s.Length == 1) s = "0" + s;
        return m + ":" + s;
    }

    public string toHour(int arg1)
    {
        int rest = arg1;
        int hours = 0, minutes = 0, seconds = 0;
        if (rest >= 3600) { hours = (int)Mathf.Floor(rest / 3600f); rest -= hours * 3600; }
        if (rest >= 60) { minutes = (int)Mathf.Floor(rest / 60f); rest -= minutes * 60; }
        if (rest >= 1) { seconds = rest; rest -= seconds; }
        string h = "" + hours, m = "" + minutes, s = "" + seconds;
        if (h.Length == 1) h = "0" + h;
        if (m.Length == 1) m = "0" + m;
        if (s.Length == 1) s = "0" + s;
        return h + ":" + m + ":" + s;
    }

    public string StripExt(string arg1)
    {
        int i = arg1.lastIndexOf(".");
        if (i != -1 && arg1.indexOf("/", i + 1) == -1 && arg1.indexOf("\\", i + 1) == -1) return arg1.slice(0, i);
        return arg1;
    }

    public string StripDir(string arg1)
    {
        int i = arg1.lastIndexOf("/");
        if (i == -1) i = arg1.lastIndexOf("\\");
        return i != -1 ? arg1.slice(i + 1) : arg1;
    }

    public string StripAll(string arg1) => StripDir(StripExt(arg1));

    public string ExtractDir(string arg1)
    {
        int i = arg1.lastIndexOf("/");
        if (i == -1) i = arg1.lastIndexOf("\\");
        return i != -1 ? arg1.slice(0, i) : "";
    }

    public string ExtractExt(string arg1)
    {
        int i = arg1.lastIndexOf(".");
        if (i != -1 && arg1.indexOf("/", i + 1) == -1 && arg1.indexOf("\\", i + 1) == -1) return arg1.slice(i + 1);
        return "";
    }
}
