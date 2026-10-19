// Ported from Settings.as (decompiled). Reads the settings XML embedded in the SWF.
using System;
using System.Collections.Generic;
using Starlite;

public class Settings
{
    /// <summary>AS3: the [Embed] class Settings_settingXm. Here: the embedded resource symbol name.</summary>
    public static string settingXm = "Settings_settingXm";
    public static XML settingXML;
    public static int productLength = 7;
    public static string Name;
    public static string Version;
    public static string packageName;
    public static string textureSize;
    public static string embedAssets;
    public static string encryptAssets;
    public static string handleDeactivate;
    public static int publishingLength = 4;
    public static string platform;
    public static string publisher;
    public static string networkAds;
    public static string externalAds;
    public static int debugLength = 1;
    public static string showStats;
    public static int supportLength = 3;
    public static string devName;
    public static string devWeb;
    public static string devSupport;

    public Settings()
    {
    }

    public static void initSetting()
    {
        // AS3: new settingXm() as ByteArray -> readUTFBytes -> new XML(...)
        XML xml = AS3.embeddedXml(settingXm);
        settingXML = xml;
        readXML();
    }

    public static void readXML()
    {
        // The root has a default xmlns (http://origamingmedia.com); children() ignores comments like E4X.
        Name = settingXML.children()[0].children()[0];
        Version = settingXML.children()[0].children()[1];
        packageName = settingXML.children()[0].children()[2];
        textureSize = settingXML.children()[0].children()[3];
        embedAssets = settingXML.children()[0].children()[4];
        encryptAssets = settingXML.children()[0].children()[5];
        handleDeactivate = settingXML.children()[0].children()[6];
        platform = settingXML.children()[1].children()[0];
        publisher = settingXML.children()[1].children()[1];
        networkAds = settingXML.children()[1].children()[2];
        externalAds = settingXML.children()[1].children()[3];
        showStats = settingXML.children()[2].children()[0];
        devName = settingXML.children()[3].children()[0];
        devWeb = settingXML.children()[3].children()[1];
        devSupport = settingXML.children()[3].children()[2];
    }

    public static void onXMLLoad(Event arg1)
    {
        settingXML = new XML((string)((URLLoader)arg1.target).data);
    }
}
