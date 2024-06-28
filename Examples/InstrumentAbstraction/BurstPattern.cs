﻿using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to burst patterns using a Digital Pattern Instrument.
    /// This class, and it's methods are intended for example purposes only,
    /// and are therefore intentionally marked as internal to prevent them from be directly invoked from code outside of this project.
    /// </summary>
    internal static class BurstPattern
    {
        internal static void BurstPatternAndPublishResults(ISemiconductorModuleContext tsmContext, string[] patternPinNames, string patternName)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var patternPins = sessionManager.Digital(patternPinNames);

            patternPins.BurstPatternAndPublishResults(patternName);

            var failCount = patternPins.GetFailCount();
            tsmContext.PublishResults(failCount, "FailCount");
        }

        internal static void BurstPatternWithDynamicSourceCapture(ISemiconductorModuleContext tsmContext, string[] patternPinNames, string patternName, string captureWaveformName, string sourceWaveformName, uint[] sourceWaveformData)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var patternPins = sessionManager.Digital(patternPinNames);

            patternPins.WriteSourceWaveformBroadcast(sourceWaveformName, sourceWaveformData);

            patternPins.BurstPattern(patternName);
            SiteData<uint[]> captureData = patternPins.FetchCaptureWaveform(captureWaveformName, -1);
        }

        internal static void BurstPatternWithDynamicSourceCaptureSiteUnique(ISemiconductorModuleContext tsmContext, string[] patternPinNames, string patternName, string captureWaveformName, string sourceWaveformName)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var patternPins = sessionManager.Digital(patternPinNames);

            // Site unique data hard-coded for 4 sites for example purposes.
            var siteUniqueSrcWfmData = new SiteData<uint[]>(new uint[][]
            {
                new uint[] { 255, 88, 01 }, // Site 0 Samples
                new uint[] { 255, 88, 11 }, // Site 1 Samples
                new uint[] { 255, 88, 21 }, // Site 2 Samples
                new uint[] { 255, 77, 31 }, // Site 3 Samples
            });

            patternPins.WriteSourceWaveformSiteUnique(sourceWaveformName, siteUniqueSrcWfmData);
            patternPins.BurstPattern(patternName);

            SiteData<uint[]> captureData = patternPins.FetchCaptureWaveform(captureWaveformName, -1);
        }

        internal static void BurstPatternWithDynamicSourceCaptureSiteUniqueSeperateContexts(ISemiconductorModuleContext tsmContext, string[] patternPinNames, string patternName, string captureWaveformName, string sourceWaveformName)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var patternPins = sessionManager.Digital(patternPinNames);

            // Site unique data hard-coded for 4 sites for example purposes.
            var siteUniqueSrcWfmData = new SiteData<uint[]>(new uint[][]
            {
                new uint[] { 255, 88, 01 }, // Site 0 Samples
                new uint[] { 255, 88, 11 }, // Site 1 Samples
                new uint[] { 255, 88, 21 }, // Site 2 Samples
                new uint[] { 255, 77, 31 }, // Site 3 Samples
            });

            foreach (var siteContext in tsmContext.GetSiteSemiconductorModuleContexts())
            {
                var currentSite = siteContext.SiteNumbers.First();
                var singleSiteSessionManager = new TSMSessionManager(siteContext);
                var singleSitePatternPins = singleSiteSessionManager.Digital(patternPinNames);

                singleSitePatternPins.WriteSourceWaveformSiteUnique(sourceWaveformName, siteUniqueSrcWfmData);
                singleSitePatternPins.BurstPattern(patternName);
            }

            SiteData<uint[]> captureData = patternPins.FetchCaptureWaveform(captureWaveformName, -1);
        }
    }
}
