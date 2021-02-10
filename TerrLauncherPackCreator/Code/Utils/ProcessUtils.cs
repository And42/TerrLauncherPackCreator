using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class ProcessUtils
    {
        public static async Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<bool>();

            void ProcessOnExited(object sender, EventArgs e)
            {
                tcs.TrySetResult(true);
            }
            
            process.EnableRaisingEvents = true;
            process.Exited += ProcessOnExited;

            try
            {
                if (process.HasExited)
                    return;

                using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                    await tcs.Task.ConfigureAwait(false);
            }
            finally
            {
                process.Exited -= ProcessOnExited;
            }
        }
    }
}