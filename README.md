# SortSnatcher
This small CLI app has been created to easily sort files downloaded through the [LoliSnatcher app](https://github.com/NO-ob/LoliSnatcher_Droid) (https://github.com/NO-ob/LoliSnatcher_Droid).

# How to use
- Clone the repository
- Rename or copy `appsettings - template.json` to `appsettings.json`
- Open `SortSnatcher.sln` and run it

# Configuration

Example configuration
```
{
  "Snatcher": {
    "DirectoryPath": "C:\\SnatchFolder",
    "DryRun": true,
    "OverwriteFiles": true,
    "Tags": {
      "carlotta_(wuthering_waves)": "Carlotta",
      "reisen_udongein_inaba": "Reisen",
      "escoffier_(genshin_impact)": "Escoffier"
    }
  }
}
```

- **DirectoryPath:** Where your files are stored, the app won't seek files from any subfolder
- **DryRun:** If `true`, the app will only show what it would do. If `false`, the app will really move the files
- **OverWriteFiles**: If `true`, the app will overwrite files if they already exist in the destination folder. If `false`, the app will skip files that already exist in the destination folder.
- **Tags**: Tags to take into account. It is a list of tags followed by the destination folder.

In this example, the files with the `carlotta_(wuthering_waves)` will be moved in a subfolder named `Carlotta`. It will also create subfolders inside the `Carlotta` folder, one subfolder by `rating` value (e.g. "general", "safe", ...)