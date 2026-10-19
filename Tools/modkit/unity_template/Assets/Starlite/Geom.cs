// Starlite: a minimal Starling-1.x compatible display framework for Unity.
// Geometry types mirror flash.geom (y axis points down, angles in radians).
using System;

namespace Starlite
{
    public class Point
    {
        public float x, y;
        public Point(float x = 0, float y = 0) { this.x = x; this.y = y; }
        public float length => (float)Math.Sqrt(x * x + y * y);
        public Point clone() => new Point(x, y);
        public Point add(Point p) => new Point(x + p.x, y + p.y);
        public Point subtract(Point p) => new Point(x - p.x, y - p.y);
        public void setTo(float x, float y) { this.x = x; this.y = y; }
        public void normalize(float thickness) { float l = length; if (l > 0) { x = x / l * thickness; y = y / l * thickness; } }
        public static float distance(Point a, Point b) { float dx = a.x - b.x, dy = a.y - b.y; return (float)Math.Sqrt(dx * dx + dy * dy); }
        public static Point interpolate(Point a, Point b, float f) => new Point(b.x + (a.x - b.x) * f, b.y + (a.y - b.y) * f);
        public static Point polar(float len, float angle) => new Point(len * (float)Math.Cos(angle), len * (float)Math.Sin(angle));
        public bool equals(Point p) => p != null && p.x == x && p.y == y;
        public override string ToString() => $"(x={x}, y={y})";
    }

    public class Rectangle
    {
        public float x, y, width, height;
        public Rectangle(float x = 0, float y = 0, float width = 0, float height = 0) { this.x = x; this.y = y; this.width = width; this.height = height; }
        public float left { get => x; set { width += x - value; x = value; } }
        public float right { get => x + width; set => width = value - x; }
        public float top { get => y; set { height += y - value; y = value; } }
        public float bottom { get => y + height; set => height = value - y; }
        public Point topLeft => new Point(x, y);
        public Point bottomRight => new Point(x + width, y + height);
        public bool isEmpty() => width <= 0 || height <= 0;
        public Rectangle clone() => new Rectangle(x, y, width, height);
        public void setTo(float x, float y, float w, float h) { this.x = x; this.y = y; width = w; height = h; }
        public bool contains(float px, float py) => px >= x && py >= y && px < x + width && py < y + height;
        public bool containsPoint(Point p) => contains(p.x, p.y);
        public bool containsRect(Rectangle r) => r.x >= x && r.y >= y && r.right <= right && r.bottom <= bottom;
        public void inflate(float dx, float dy) { x -= dx; width += 2 * dx; y -= dy; height += 2 * dy; }
        public void offset(float dx, float dy) { x += dx; y += dy; }
        public bool intersects(Rectangle r) => r.x < right && r.right > x && r.y < bottom && r.bottom > y;
        public Rectangle intersection(Rectangle r)
        {
            float l = Math.Max(x, r.x), t = Math.Max(y, r.y), rr = Math.Min(right, r.right), b = Math.Min(bottom, r.bottom);
            return (rr > l && b > t) ? new Rectangle(l, t, rr - l, b - t) : new Rectangle();
        }
        public Rectangle union(Rectangle r)
        {
            if (isEmpty()) return r.clone();
            if (r.isEmpty()) return clone();
            float l = Math.Min(x, r.x), t = Math.Min(y, r.y);
            return new Rectangle(l, t, Math.Max(right, r.right) - l, Math.Max(bottom, r.bottom) - t);
        }
        public override string ToString() => $"(x={x}, y={y}, w={width}, h={height})";
    }

    /// <summary>2D affine matrix: [a c tx; b d ty] (flash.geom.Matrix semantics).</summary>
    public class Matrix
    {
        public float a = 1, b, c, d = 1, tx, ty;
        public Matrix() { }
        public Matrix(float a, float b, float c, float d, float tx, float ty) { this.a = a; this.b = b; this.c = c; this.d = d; this.tx = tx; this.ty = ty; }
        public Matrix clone() => new Matrix(a, b, c, d, tx, ty);
        public void identity() { a = d = 1; b = c = tx = ty = 0; }
        public void copyFrom(Matrix m) { a = m.a; b = m.b; c = m.c; d = m.d; tx = m.tx; ty = m.ty; }
        /// <summary>this = this * m (apply this first, then m) - flash "concat".</summary>
        public void concat(Matrix m)
        {
            float na = a * m.a + b * m.c, nb = a * m.b + b * m.d;
            float nc = c * m.a + d * m.c, nd = c * m.b + d * m.d;
            float ntx = tx * m.a + ty * m.c + m.tx, nty = tx * m.b + ty * m.d + m.ty;
            a = na; b = nb; c = nc; d = nd; tx = ntx; ty = nty;
        }
        public void invert()
        {
            float det = a * d - b * c;
            if (det == 0) { identity(); tx = ty = float.NaN; return; }
            float ia = d / det, ib = -b / det, ic = -c / det, id = a / det;
            float itx = -(ia * tx + ic * ty), ity = -(ib * tx + id * ty);
            a = ia; b = ib; c = ic; d = id; tx = itx; ty = ity;
        }
        public Point transformPoint(Point p) => new Point(a * p.x + c * p.y + tx, b * p.x + d * p.y + ty);
        public void transform(float x, float y, out float ox, out float oy) { ox = a * x + c * y + tx; oy = b * x + d * y + ty; }
        public void translate(float dx, float dy) { tx += dx; ty += dy; }
        public void scale(float sx, float sy) { a *= sx; b *= sy; c *= sx; d *= sy; tx *= sx; ty *= sy; }
        public void rotate(float ang)
        {
            float cos = (float)Math.Cos(ang), sin = (float)Math.Sin(ang);
            concat(new Matrix(cos, sin, -sin, cos, 0, 0));
        }
    }

    public static class Color
    {
        public const uint WHITE = 0xffffff, SILVER = 0xc0c0c0, GRAY = 0x808080, BLACK = 0x000000, RED = 0xff0000,
            MAROON = 0x800000, YELLOW = 0xffff00, OLIVE = 0x808000, LIME = 0x00ff00, GREEN = 0x008000, AQUA = 0x00ffff,
            TEAL = 0x008080, BLUE = 0x0000ff, NAVY = 0x000080, FUCHSIA = 0xff00ff, PURPLE = 0x800080;
        public static int getRed(uint c) => (int)((c >> 16) & 0xff);
        public static int getGreen(uint c) => (int)((c >> 8) & 0xff);
        public static int getBlue(uint c) => (int)(c & 0xff);
        public static uint rgb(int r, int g, int b) => (uint)((r << 16) | (g << 8) | b);
        internal static UnityEngine.Color ToUnity(uint rgb, float alpha = 1f) =>
            new UnityEngine.Color(((rgb >> 16) & 0xff) / 255f, ((rgb >> 8) & 0xff) / 255f, (rgb & 0xff) / 255f, alpha);
    }
}
