using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageServer
{
    internal static class FileAccessor
    {
        private static object _masterLock = new object();
        private static List<string> _fileLocks = new List<string>();

        public static async Task RunAction(string fileName, Func<Task> action)
        {
            bool canAccessFile = false;

            while (true)
            {
                lock (_masterLock)
                {
                    if (!_fileLocks.Contains(fileName))
                    {
                        _fileLocks.Add(fileName);
                        canAccessFile = true;
                    }
                }

                if (canAccessFile)
                {
                    break;
                }
                else
                {
                    await Task.Delay(100);
                }
            }

            try
            {
                await Task.Run(action);
            }
            finally
            {
                lock (_masterLock)
                {
                    _fileLocks.Remove(fileName);
                }
            }
        }
    }
}
