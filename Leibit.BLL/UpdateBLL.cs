using Leibit.Core.Common;
using Leibit.Core.Exceptions;
using Leibit.Entities.Updates;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Leibit.BLL
{
    public class UpdateBLL
    {

        #region - Needs -
        private readonly string m_UpdateExe;
        private readonly string m_UpdateUrl;
        #endregion

        #region - Const -
        private const string UPDATE_EXE = "update.exe";
        #endregion

        #region - Ctor -
        public UpdateBLL(string updateUrl)
        {
            m_UpdateExe = __TryFindUpdateExe();
            m_UpdateUrl = updateUrl;
        }
        #endregion

        #region - Events -
        public event EventHandler<int> UpdateProgress;
        #endregion

        #region - Internal methods -

        #region [CheckForUpdates]
        public async Task<OperationResult<CheckForUpdatesResult>> CheckForUpdates()
        {
            try
            {
                if (m_UpdateExe == null)
                    return OperationResult<CheckForUpdatesResult>.Ok(new CheckForUpdatesResult()); // App not managed by Squirrel.

                var processStartInfo = __GetProcessStartInfoForUpdateExe();
                processStartInfo.Arguments = $"--checkForUpdate={m_UpdateUrl}";

                var process = Process.Start(processStartInfo);
                process.WaitForExit();
                await __ValidateProcessTerminatedSuccessfully(process);

                var stdOut = await process.StandardOutput.ReadToEndAsync();
                var jsonStart = stdOut.IndexOf('{');

                if (jsonStart < 0)
                    return OperationResult<CheckForUpdatesResult>.Fail("Invalid output of update.exe"); // Something went wrong :(

                var json = stdOut.Substring(jsonStart);
                var result = JsonConvert.DeserializeObject<CheckForUpdatesResult>(json);
                return OperationResult<CheckForUpdatesResult>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<CheckForUpdatesResult>.Fail(ex.ToString());
            }
        }
        #endregion

        #region [Update]
        public async Task<OperationResult<bool>> Update()
        {
            try
            {
                if (m_UpdateExe == null)
                    return OperationResult<bool>.Ok(false); // App not managed by Squirrel.

                var processStartInfo = __GetProcessStartInfoForUpdateExe();
                processStartInfo.Arguments = $"--update={m_UpdateUrl}";

                var process = Process.Start(processStartInfo);
                string line;

                do
                {
                    line = await process.StandardOutput.ReadLineAsync();

                    if (int.TryParse(line, out int progress))
                        UpdateProgress?.Invoke(this, progress);
                }
                while (line.IsNotNullOrEmpty());

                process.WaitForExit();
                await __ValidateProcessTerminatedSuccessfully(process);
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.ToString());
            }
        }
        #endregion

        #region [RestartApp]
        public OperationResult<bool> RestartApp()
        {
            try
            {
                if (m_UpdateExe == null)
                    return OperationResult<bool>.Ok(false); // App not managed by Squirrel.

                var exeName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
                var processStartInfo = __GetProcessStartInfoForUpdateExe();
                processStartInfo.Arguments = $"--processStartAndWait=\"{exeName}\"";
                Process.Start(processStartInfo);
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.ToString());
            }
        }
        #endregion

        #endregion

        #region - Private helper -

        #region [__TryFindUpdateExe]
        private string __TryFindUpdateExe()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var dir = Path.GetDirectoryName(assembly.Location);
            var updateExe = Path.Combine(dir, UPDATE_EXE);

            if (File.Exists(updateExe))
                return updateExe;

            updateExe = Path.Combine(dir, "..", UPDATE_EXE);

            if (File.Exists(updateExe))
                return updateExe;

            return null;
        }
        #endregion

        #region [__GetProcessStartInfoForUpdateExe]
        private ProcessStartInfo __GetProcessStartInfoForUpdateExe()
        {
            var processStartInfo = new ProcessStartInfo(m_UpdateExe);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.WorkingDirectory = Path.GetDirectoryName(m_UpdateExe);
            return processStartInfo;
        }
        #endregion

        #region [__ValidateProcessTerminatedSuccessfully]
        private async Task __ValidateProcessTerminatedSuccessfully(Process process)
        {
            if (process.ExitCode == 0)
                return;

            var stdErr = await process.StandardError.ReadToEndAsync();
            var stdOut = await process.StandardOutput.ReadToEndAsync();

            if (stdErr.IsNotNullOrWhiteSpace())
                throw new OperationFailedException($"update.exe failed with exit code {process.ExitCode}. STDERR: {stdErr}");
            else if (stdOut.IsNotNullOrWhiteSpace())
                throw new OperationFailedException($"update.exe failed with exit code {process.ExitCode}. STDOUT: {stdOut}");
            else
                throw new OperationFailedException($"update.exe failed with exit code {process.ExitCode}.");
        }
        #endregion

        #endregion

    }
}