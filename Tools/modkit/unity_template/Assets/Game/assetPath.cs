// Ported from rudi.assetPath: queues of asset file names consumed by loaders.
using System.Collections.Generic;
using Starlite;

public class assetPath
{
    public static List<object> imageAr = new List<object>();
    public static List<object> xmlAr = new List<object>();
    public static List<object> soundAr = new List<object>();
    public static List<object> soundNameAr = new List<object>();
    public static List<object> skeletonAr = new List<object>();
    public static List<object> skeletonXmlAr = new List<object>();
    public static List<object> skeletonTextureAr = new List<object>();
    public static int imageArCnt = 0;
    public static int xmlArCnt = 0;
    public static int soundArCnt = 0;
    public static int soundNameArCnt = 0;
    public static int skeletonArCnt = 0;
    public static int skeletonXmlArCnt = 0;
    public static int skeletonTextureArCnt = 0;
    public static string graphicsPath;
    public static string soundsPath;
    public static string skeletonPath;
    public static string tName;

    public static void start(string arg1, string arg2)
    {
        graphicsPath = arg1;
        soundsPath = arg2;
    }

    public static void startSkeleton(string arg1) => skeletonPath = arg1;

    // Encrypted asset names (".OrigamingI" etc.) are never used by this build (EncryptAssets=false).
    static string Next(List<object> ar, ref int cnt, string basePath, string encExt)
    {
        if (cnt < ar.Count && ar.Count != 0)
        {
            cnt++;
            tName = Global.getInstance().assetEncrypt
                ? maxCrypto.getInstance().StripExt(AS3.str(ar[cnt - 1])) + encExt
                : AS3.str(ar[cnt - 1]);
            return basePath + tName;
        }
        return "";
    }

    public static string getGraphics() => Next(imageAr, ref imageArCnt, graphicsPath, ".OrigamingI");
    public static string getSkeleton() => Next(skeletonAr, ref skeletonArCnt, skeletonPath, ".OrigamingSk");
    public static string getSkeletonTextureXml() => Next(skeletonTextureAr, ref skeletonTextureArCnt, skeletonPath, ".OrigamingSkTX");
    public static string getSkeletonXml() => Next(skeletonXmlAr, ref skeletonXmlArCnt, skeletonPath, ".OrigamingSkX");
    public static string getXml() => Next(xmlAr, ref xmlArCnt, graphicsPath, ".OrigamingX");
    public static string getSounds() => Next(soundAr, ref soundArCnt, soundsPath, ".OrigamingS");

    public static string getSoundsName()
    {
        if (soundNameArCnt < soundNameAr.Count && soundNameAr.Count != 0)
        {
            soundNameArCnt++;
            return AS3.str(soundNameAr[soundNameArCnt - 1]);
        }
        return "";
    }

    public static void reset()
    {
        imageArCnt = xmlArCnt = soundArCnt = soundNameArCnt = skeletonArCnt = skeletonTextureArCnt = skeletonXmlArCnt = 0;
        imageAr = new List<object>();
        xmlAr = new List<object>();
        soundAr = new List<object>();
        soundNameAr = new List<object>();
        skeletonAr = new List<object>();
        skeletonTextureAr = new List<object>();
        skeletonXmlAr = new List<object>();
    }
}
