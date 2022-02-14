namespace zblesk_web.Models;

public abstract class DataHeader
{
    public string OwnerId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }

}
