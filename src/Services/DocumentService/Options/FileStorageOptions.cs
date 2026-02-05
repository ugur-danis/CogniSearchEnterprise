using System.ComponentModel.DataAnnotations;

namespace DocumentService.Options;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    [Required]
    public string UploadPath { get; set; } = string.Empty;

    [Required]
    public string AllowedExtensions { get; set; } = string.Empty;
}
