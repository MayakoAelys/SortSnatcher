using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace SortSnatcher
{
    internal class Program
    {
        static ConfigurationSnatcher? _configSnatcher;

        static void Main(string[] args)
        {
            using (IHost host = Host.CreateApplicationBuilder(args).Build())
            {
                IConfiguration configuration =
                    new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .AddEnvironmentVariables()
                        .Build();

                _configSnatcher = 
                    configuration.GetRequiredSection("Snatcher").Get<ConfigurationSnatcher>();

                if (_configSnatcher == null) 
                    throw new Exception("Configuration issue. Please check if appsettings.json exists and is correctly formatted.");

                if (AskConfirmation())
                    StartSorting();
                else
                    Console.WriteLine("... Cancelling.");

                Console.WriteLine("\n\n=== END ===");
            }
        }

        static bool AskConfirmation()
        {
            string answer = string.Empty;

            Console.WriteLine("=== SortSnatcher ===");
            Console.WriteLine("Config: ");
            Console.WriteLine($"    DirectoryPath: { _configSnatcher.DirectoryPath }");
            Console.WriteLine($"    DryRun: {_configSnatcher.DryRun}");
            Console.WriteLine("    Tags:");

            foreach (var tag in _configSnatcher.Tags)
            {
                Console.WriteLine($"        - { tag.Key } -> Folder: { tag.Value }");
            }

            Console.WriteLine("");
            Console.Write("Do you want to continue? (y/N) ");

            answer = Console.ReadLine().ToLower();

            if ("y".Equals(answer.ToLower()))
                return true;
            
            return false;
        }

        static void StartSorting()
        {
            if (_configSnatcher == null)
                throw new Exception("Configuration issue. Please check if appsettings.json exists and is correctly formatted.");

            DirectoryInfo snatcherFolder = GetSnatcherFolder();

            List<FileToMove> filesToMove = GetFilesToMove(snatcherFolder);

            Console.WriteLine($"\n\n{filesToMove.Count} file(s) to move...");

            MoveFiles(filesToMove, snatcherFolder);
        }

        /// <summary>
        /// Get all json files from the folder
        /// </summary>
        private static DirectoryInfo GetSnatcherFolder()
        {
            DirectoryInfo snatcherFolder = new DirectoryInfo(_configSnatcher.DirectoryPath);

            if (!snatcherFolder.Exists)
                throw new Exception($"Snatcher folder not found or permission issue (path: {snatcherFolder})");

            return snatcherFolder;
        }

        /// <summary>
        /// Get files with a corresponding tag from the config
        /// </summary>
        private static List<FileToMove> GetFilesToMove(DirectoryInfo snatcherFolder)
        {
            List<FileInfo> allFiles = snatcherFolder.GetFiles().ToList();

            List<FileInfo> jsonFiles =
                allFiles
                    .Where(file => file.Extension.Equals(".json"))
                    .ToList();

            // Check if there is a corresponding picture with the same name (assuming it is any extension other than json)
            var filesToMove = new List<FileToMove>();

            foreach (FileInfo jsonFile in jsonFiles)
            {
                string filenameWithoutExtension = jsonFile.Name.Substring(0, jsonFile.Name.Length - 5);

                Console.WriteLine($"--> {filenameWithoutExtension}");

                // Get the corresponding picture file
                FileInfo? imageFile =
                    allFiles.FirstOrDefault(
                        file => !file.Extension.Equals(".json") &&
                                 file.Name.StartsWith(filenameWithoutExtension));

                if (imageFile == null)
                {
                    Console.WriteLine(@"    /!\ Picture file not found");
                    continue;
                }

                Console.WriteLine("    - Picture file found");

                string jsonFileContent = File.ReadAllText(jsonFile.FullName);

                ImageInfo? jsonInfo = JsonSerializer.Deserialize<ImageInfo>(jsonFileContent);

                if (jsonInfo == null)
                    throw new Exception(@"    /!\ Couldn't read json info");

                KeyValuePair<string, string>? selectedTag =
                    _configSnatcher.Tags.FirstOrDefault(
                        configTag => jsonInfo.Tags.Any(fileTag => fileTag.Equals(configTag.Key)));

                jsonInfo.Tags.FirstOrDefault(tag =>
                    _configSnatcher.Tags.Any(configTag => configTag.Key.Equals(tag)));

                if (new KeyValuePair<string, string>().Equals(selectedTag.Value))
                {
                    Console.WriteLine(@"    /!\ No corresponding tag, skipping...");
                    continue;
                }

                filesToMove.Add(new FileToMove
                {
                    ImageFile = imageFile,
                    JsonFile = jsonFile,
                    SelectedTag = selectedTag.Value,
                    FileName = filenameWithoutExtension,
                    Rating = jsonInfo.Rating
                });
            }

            return filesToMove;
        }

        private static void MoveFiles(List<FileToMove> filesToMove, DirectoryInfo snatcherFolder)
        {
            bool dryRun = _configSnatcher?.DryRun ?? true;
            string dryRunTag = dryRun ? "[DRY]" : "";

            foreach (FileToMove fileToMove in filesToMove)
            {
                Console.WriteLine($"- {fileToMove.FileName}");
                Console.WriteLine("    Moving file...");
                Console.WriteLine($"    Tag: {fileToMove.SelectedTag}");
                Console.WriteLine($"    Rating: {fileToMove.Rating}");

                // Ensure destination folders existence
                // Tag folder (e.g.: "/Carlotta")
                string tagFolderPath = Path.Combine(snatcherFolder.FullName, fileToMove.SelectedTag.Value);

                var tagFolder = new DirectoryInfo(tagFolderPath);

                if (!tagFolder.Exists)
                {
                    Console.WriteLine($"    { dryRunTag } Tag folder doesn't exist, creating it...");

                    if (!dryRun)
                        tagFolder.Create();
                }

                // Rating folder (e.g.: safe, general)
                string ratingFolderPath = Path.Combine(tagFolder.FullName, fileToMove.Rating);

                var ratingFolder = new DirectoryInfo(ratingFolderPath);

                if (!ratingFolder.Exists)
                {
                    Console.WriteLine($"    { dryRunTag } Rating folder doesn't exist, creating it...");
                    
                    if (!dryRun)
                        ratingFolder.Create();
                }

                // Move files
                string jsonOldPath = fileToMove.JsonFile.FullName;
                string jsonNewPath = Path.Combine(ratingFolder.FullName, fileToMove.JsonFile.Name);
                string imageOldPath = fileToMove.ImageFile.FullName;
                string imageNewPath = Path.Combine(ratingFolder.FullName, fileToMove.ImageFile.Name);

                if (!dryRun)
                {
                    File.Move(jsonOldPath, jsonNewPath);
                    File.Move(imageOldPath, imageNewPath);
                }

                Console.WriteLine($"    ! { dryRunTag } Files moved");
            }
        }
    }
}
