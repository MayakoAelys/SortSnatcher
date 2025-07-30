namespace SortSnatcher
{
    internal class FileToMove
    {
        /// <summary>
        /// Image JSON file
        /// </summary>
        public required FileInfo JsonFile { get; set; }
        /// <summary>
        /// Image file
        /// </summary>
        public required FileInfo ImageFile { get; set; }
        /// <summary>
        /// Filename without extension
        /// </summary>
        public required string FileName { get; set; }
        /// <summary>
        /// Rating categorie
        /// </summary>
        public required string Rating { get; set; }
        /// <summary>
        /// Key: tag, Value: folder name
        /// </summary>
        public required KeyValuePair<string, string> SelectedTag { get; set; }
    }
}
