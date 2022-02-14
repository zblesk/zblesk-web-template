using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace zblesk_web.Models;

[Table("Images")]
public class StoredImage
{
    public enum ImageKind { Unknown, BookCover, ProfilePic }

    [Key]
    public string FileName { get; set; }
    public byte[] FileContents { get; set; }
    public string Extension { get; set; }
    public ImageKind Kind { get; set; }
}
