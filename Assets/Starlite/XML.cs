// E4X-flavoured XML access so ported ActionScript reads naturally:
//   AS3:  dishXML.dish[i].@region          C#: dishXML["dish"][i]["@region"]   (or .attribute("region"))
//   AS3:  levelSetXML.day[i].time          C#: levelSetXML["day"][i]["time"]
//   AS3:  xml.day.length()                 C#: xml["day"].length()
// XMLList converts implicitly to string; use toInt()/toFloat()/toBool() for numbers.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Starlite
{
    public class XML
    {
        internal readonly XElement el;      // element node
        internal readonly string value;     // attribute / text node value (when el == null)
        internal readonly string nodeName;

        public XML(string text)
        {
            var doc = XDocument.Parse(Lenient(text ?? ""), LoadOptions.None);
            el = doc.Root;
            nodeName = el?.Name.LocalName;
        }

        internal XML(XElement e) { el = e; nodeName = e.Name.LocalName; }
        internal XML(string name, string val) { nodeName = name; value = val; }

        /// <summary>Flash accepts "--" inside comments; System.Xml does not.</summary>
        public static string Lenient(string s)
        {
            if (s.Length > 0 && s[0] == '﻿') s = s.Substring(1);
            if (!s.Contains("<!--")) return s;
            var sb = new StringBuilder(s.Length);
            int i = 0;
            while (true)
            {
                int a = s.IndexOf("<!--", i, StringComparison.Ordinal);
                if (a < 0) { sb.Append(s, i, s.Length - i); break; }
                int b = s.IndexOf("-->", a + 4, StringComparison.Ordinal);
                if (b < 0) { sb.Append(s, i, s.Length - i); break; }
                sb.Append(s, i, a + 4 - i);
                string body = s.Substring(a + 4, b - a - 4).Replace("--", "- ");
                if (body.EndsWith("-")) body = body.Substring(0, body.Length - 1) + " ";
                sb.Append(body).Append("-->");
                i = b + 3;
            }
            return sb.ToString();
        }

        /// <summary>Child elements by name, or attributes with "@name"; "*" for all elements.</summary>
        public XMLList this[string name]
        {
            get
            {
                if (el == null) return new XMLList();
                if (name.StartsWith("@")) return attribute(name.Substring(1));
                if (name == "*") return elements();
                return new XMLList(el.Elements().Where(e => e.Name.LocalName == name).Select(e => new XML(e)));
            }
        }

        public XMLList attribute(string name)
        {
            if (el == null) return new XMLList();
            var a = el.Attributes().FirstOrDefault(x => x.Name.LocalName == name);
            return a == null ? new XMLList() : new XMLList(new[] { new XML(name, a.Value) });
        }

        public XMLList attributes() => el == null ? new XMLList() : new XMLList(el.Attributes().Select(a => new XML(a.Name.LocalName, a.Value)));
        public XMLList child(string name) => this[name];
        public XMLList elements(string name = "*") =>
            el == null ? new XMLList() : new XMLList(el.Elements().Where(e => name == "*" || e.Name.LocalName == name).Select(e => new XML(e)));

        /// <summary>Element and text children (E4X children()).</summary>
        public XMLList children()
        {
            if (el == null) return new XMLList();
            var list = new List<XML>();
            foreach (var n in el.Nodes())
            {
                if (n is XElement e) list.Add(new XML(e));
                else if (n is XText t && t.Value.Trim().Length > 0) list.Add(new XML("#text", t.Value));
            }
            return new XMLList(list);
        }

        /// <summary>E4X attribute write: AS3 "node.@attr = value".</summary>
        public void setAttribute(string name, string value) => el?.SetAttributeValue(name, value);

        public string name() => nodeName;
        public string localName() => nodeName;
        public int length() => 1;
        public bool hasSimpleContent() => el == null || !el.HasElements;
        public string text() => el != null ? string.Concat(el.Nodes().OfType<XText>().Select(t => t.Value)) : value;
        public string toXMLString() => el != null ? el.ToString() : value;
        public override string ToString() => el == null ? value : (el.HasElements ? el.ToString() : el.Value);
        public string toString() => ToString();
        public int toInt() => AS3.toInt(ToString());
        public float toFloat() => AS3.toNumber(ToString());
        public bool toBool() => ToString() == "true";
        public static implicit operator string(XML x) => x?.ToString();
    }

    public class XMLList : IEnumerable<XML>
    {
        readonly List<XML> items;
        public XMLList() { items = new List<XML>(); }
        public XMLList(IEnumerable<XML> src) { items = src.ToList(); }

        public XML this[int i] => i >= 0 && i < items.Count ? items[i] : null;

        public XMLList this[string name]
        {
            get
            {
                var all = new List<XML>();
                foreach (var x in items) all.AddRange(x[name]);
                return new XMLList(all);
            }
        }

        public XMLList attribute(string name)
        {
            var all = new List<XML>();
            foreach (var x in items) all.AddRange(x.attribute(name));
            return new XMLList(all);
        }

        public XMLList children()
        {
            var all = new List<XML>();
            foreach (var x in items) all.AddRange(x.children());
            return new XMLList(all);
        }

        public XMLList elements(string name = "*")
        {
            var all = new List<XML>();
            foreach (var x in items) all.AddRange(x.elements(name));
            return new XMLList(all);
        }

        public int length() => items.Count;
        public string text() => string.Concat(items.Select(x => x.text()));
        public override string ToString() => items.Count == 1 ? items[0].ToString() : string.Concat(items.Select(x => x.toXMLString()));
        public string toString() => ToString();
        public int toInt() => AS3.toInt(ToString());
        public float toFloat() => AS3.toNumber(ToString());
        public bool toBool() => ToString() == "true";
        public static implicit operator string(XMLList l) => l?.ToString();
        public IEnumerator<XML> GetEnumerator() => items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    }
}
