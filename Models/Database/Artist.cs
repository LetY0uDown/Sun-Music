namespace Models.Database;

public partial class Artist : Entity
{
    public Artist ()
    {
        Albums = new HashSet<Album>();
    }

    public string Title { get; set; } = null!;
    public DateTime StartYear { get; set; }
    public DateTime EndYear { get; set; }
    public string Members { get; set; } = null!;
    public byte[] ImageBytes { get; set; } = null!;

    public virtual ICollection<Album> Albums { get; set; }
}
