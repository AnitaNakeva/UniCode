using System.Diagnostics;

namespace UniCodeProject.API.Services
{
    public class DockerExecutionService
    {
        public async Task<string> ExecuteCodeAsync(string code, string language)
        {
            try
            {
                string basePath = @"C:\Users\Ita\source\repos\UniCodeProject.API\UniCodeProject.API\DockerExecution";
                string fileName = "";
                string imageName = "";
                string filePath = "";
                string workingDir = "";

                switch (language.ToLower())
                {
                    case "csharp":
                        fileName = "main.cs";
                        imageName = "csharp_execution";
                        workingDir = Path.Combine(basePath, "csharp");
                        break;
                    case "python":
                        fileName = "main.py";
                        imageName = "python_execution";
                        workingDir = Path.Combine(basePath, "python");
                        break;
                    case "java":
                        fileName = "Main.java";
                        imageName = "java_execution";
                        workingDir = Path.Combine(basePath, "java");
                        break;
                    case "cpp":
                        fileName = "main.cpp";
                        imageName = "cpp_execution";
                        workingDir = Path.Combine(basePath, "cpp");
                        break;
                    case "javascript":
                        fileName = "main.js";
                        imageName = "js_execution";
                        workingDir = Path.Combine(basePath, "js");
                        break;
                    default:
                        return $"Unsupported language: {language}";
                }

                filePath = Path.Combine(workingDir, fileName);
                await File.WriteAllTextAsync(filePath, code);

                var buildInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"build -t {imageName} .",
                    WorkingDirectory = workingDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var buildProcess = Process.Start(buildInfo);
                string buildOut = await buildProcess.StandardOutput.ReadToEndAsync();
                string buildErr = await buildProcess.StandardError.ReadToEndAsync();
                await buildProcess.WaitForExitAsync();

                if (buildProcess.ExitCode != 0)
                    return $"Build failed:\n{buildErr}";

                var runInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"run --rm {imageName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var runProcess = Process.Start(runInfo);
                string output = await runProcess.StandardOutput.ReadToEndAsync();
                string error = await runProcess.StandardError.ReadToEndAsync();
                await runProcess.WaitForExitAsync();

                return $"Output: {output}\nError: {error}";
                return string.IsNullOrWhiteSpace(error) ? output.Trim() : error.Trim();
            }
            catch(Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
    }
}