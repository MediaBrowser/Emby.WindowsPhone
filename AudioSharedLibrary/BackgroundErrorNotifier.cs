// -----------------------------------------------------------------------
// <copyright file="BackgroundErrorNotifier.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this sample source code is subject to the terms of the Microsoft
// license agreement under which you licensed this sample source code. If
// you did not accept the terms of the license agreement, you are not
// authorized to use this sample source code. For the terms of the license,
// please see the license agreement between you and Microsoft or, if applicable,
// see the LICENSE.RTF on your install media or the root of your tools installation.
// THE SAMPLE SOURCE CODE IS PROVIDED "AS IS", WITH NO WARRANTIES OR INDEMNITIES.
// </copyright>
// -----------------------------------------------------------------------

namespace AudioSharedLibrary
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Threading;

    public static class BackgroundErrorNotifier
    {
        // create mutex on first use
        private static readonly Mutex mutex = new Mutex(false, "BackgroundErrorNotifierMutex");
        
        // this is our log file in Isolated Storage
        private const string ErrorFile = "ErrorFile.xml";

        public static void AddError(Exception exception)
        {            
            string errorDetail = "";
            string exceptionMessage = exception.Message;
            if (exceptionMessage == "-2147012889") 
            {
                // 80072EE7
                errorDetail = "Remote server not available";
            }
            else if (exceptionMessage == "-2147012696") 
            {
                // 80072FA8
                errorDetail = "No available network connection";
            }
            else if (exceptionMessage == "-1072889830") 
            {
                // C00D001A
                errorDetail = "Can't find media file";
            }
            else
            {
                // Note: if server is not found it returns hex 80004005(E_FAIL) 
                // this could also be a number of other errors
                errorDetail = "Unknown error";
            }

            string fullErrorString = errorDetail;

            // write to debugger (DEBUG mode)
            Debug.WriteLine(fullErrorString);

            // take the mutex
            mutex.WaitOne();
            try
            {
                // write to file
                using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var stream = iso.OpenFile(ErrorFile, FileMode.Create))
                    {
                        using (var writer = new BinaryWriter(stream))
                        {
                            writer.Write(fullErrorString);
                        }
                    }
                }
            }
            finally
            {
                // release the mutex even if we crashed
                mutex.ReleaseMutex();
            }            
        }

        public static string GetError()
        {
            // take the mutex
            mutex.WaitOne();
            string fullErrorString = null;
            try
            {
                using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (iso.FileExists(ErrorFile))
                    {
                        using (var stream = iso.OpenFile(ErrorFile, FileMode.Open))
                        {
                            using (var reader = new BinaryReader(stream))
                            {
                                try
                                {
                                    fullErrorString = reader.ReadString();
                                }
                                catch (Exception)
                                {
                                    // bad things happened - ignore the data
                                    fullErrorString = null;
                                }
                            }                            
                        }

                        iso.DeleteFile(ErrorFile);
                    }
                }
            }
            finally
            {
                // release the mutex even if we crashed
                mutex.ReleaseMutex();
            }

            return fullErrorString;
        }
    }
}
