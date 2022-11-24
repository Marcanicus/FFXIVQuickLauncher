﻿using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;
using XIVLauncher.Common.Util;

namespace XIVLauncher.Common.Unix.Compatibility;

public static class Dxvk
{
    private static string DXVK_DOWNLOAD = "https://github.com/Sporif/dxvk-async/releases/download/1.10.1/dxvk-async-1.10.1.tar.gz";
    private static string DXVK_NAME = "dxvk-async-1.10.1";
    
    private static string DXVK_VERSION = "1.10.1";
    public static string Version
    {
        get { return DXVK_VERSION; }
        set
        {
            switch (value)
            {
                case "2.0":
                    DXVK_VERSION = "2.0";
                    break;
                case "1.10.3":
                    DXVK_VERSION = "1.10.3";
                    break;
                case "1.10.2":
                    DXVK_VERSION = "1.10.2";
                    break;
                default:
                    DXVK_VERSION = "1.10.1";
                    break;
            }
            DXVK_NAME = $"dxvk-async-{DXVK_VERSION}";
            DXVK_DOWNLOAD = $"https://github.com/Sporif/dxvk-async/releases/download/{DXVK_VERSION}/{DXVK_NAME}.tar.gz";
        }
    }

    public static async Task InstallDxvk(DirectoryInfo prefix, DirectoryInfo installDirectory)
    {
        var dxvkPath = Path.Combine(installDirectory.FullName, DXVK_NAME, "x64");

        if (!Directory.Exists(dxvkPath))
        {
            Log.Information("DXVK does not exist, downloading");
            await DownloadDxvk(installDirectory).ConfigureAwait(false);
        }

        var system32 = Path.Combine(prefix.FullName, "drive_c", "windows", "system32");
        var files = Directory.GetFiles(dxvkPath);

        foreach (string fileName in files)
        {
            File.Copy(fileName, Path.Combine(system32, Path.GetFileName(fileName)), true);
        }
    }

    private static async Task DownloadDxvk(DirectoryInfo installDirectory)
    {
        using var client = new HttpClient();
        var tempPath = Path.GetTempFileName();

        File.WriteAllBytes(tempPath, await client.GetByteArrayAsync(DXVK_DOWNLOAD));
        PlatformHelpers.Untar(tempPath, installDirectory.FullName);

        File.Delete(tempPath);
    }

    public enum DxvkHudType
    {
        [SettingsDescription("None", "Show nothing")]
        None,

        [SettingsDescription("FPS", "Only show FPS")]
        Fps,

        [SettingsDescription("Full", "Show everything")]
        Full,
    }
}