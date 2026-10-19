// Ported from TextureInfo.as (decompiled).
using Starlite;

public class TextureInfo
{
    public Rectangle region;
    public Rectangle frame;
    public bool rotated;

    public TextureInfo(Rectangle arg1, Rectangle arg2, bool arg3)
    {
        this.region = arg1;
        this.frame = arg2;
        this.rotated = arg3;
    }
}
