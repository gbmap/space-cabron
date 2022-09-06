using System.Collections;
using System.Collections.Generic;
using Gmap.Localization;
using UnityEngine;
using NUnit.Framework;

public static class LocalizationTests
{
    [Test]
    public static void SpreadsheetDownloaderDownloadsSpreadsheet()
    {
        string url = "https://docs.google.com/spreadsheets/d/14LFhH_oD3wJ3hVdMmllGarb9nqwPTA9uGexunvKqDCE/export?format=csv&usp=sharing";

        SpreadsheetDownloader downloader = new SpreadsheetDownloader(url);
        var result = downloader.Download();
        Assert.IsTrue(result.Success);
        Assert.AreNotEqual(result.RawResult.Length, 0);
        Assert.AreNotEqual(result.Result.Rows.Count, 0);
    }
}