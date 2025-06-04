using System.Diagnostics;

namespace UniCodeProject.API.Services
{
    public class DockerExecutionService
    {
        private readonly string _dockerfilePath = @"C:\Users\Ita\source\repos\UniCodeProject.API\UniCodeProject.API\DockerExecution\csharp\main.cs";
        private readonly string _imageName = "csharp_execution";

        public async Task<string> ExecuteCodeAsync(string code, string language)
        {
            if (language.ToLower() == "csharp")
            {
                await File.WriteAllTextAsync(_dockerfilePath, code);

                var startInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"build -t {_imageName} .",
                    WorkingDirectory = Path.GetDirectoryName(_dockerfilePath),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var buildProcess = Process.Start(startInfo);
                string buildOut = await buildProcess.StandardOutput.ReadToEndAsync();
                string buildErr = await buildProcess.StandardError.ReadToEndAsync();
                await buildProcess.WaitForExitAsync();

                if (buildProcess.ExitCode != 0)
                    return $"Build failed:\n{buildErr}";

                var runInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"run --rm {_imageName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var runProcess = Process.Start(runInfo);
                string output = await runProcess.StandardOutput.ReadToEndAsync();
                string error = await runProcess.StandardError.ReadToEndAsync();
                await runProcess.WaitForExitAsync();

                return string.IsNullOrWhiteSpace(error) ? output.Trim() : error.Trim();
            }

            return $"Unsupported language: {language}";
        }
    }
}