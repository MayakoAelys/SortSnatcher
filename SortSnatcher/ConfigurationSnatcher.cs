namespace SortSnatcher
{
    internal class ConfigurationSnatcher
    {
        /// <summary>
        /// Snatcher base folder
        /// </summary>
        public string DirectoryPath { get; set; }
        /// <summary>
        /// True: won't move, only show what would be done
        /// </summary>
        public bool DryRun { get; set; }
        /// <summary>
        /// Booru tag - Directory name
        /// </summary>
        public Dictionary<string, string> Tags { get; set; }
    }
}
