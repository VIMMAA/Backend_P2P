using Domain.Abstractions;

public class ReportModel : Entity
{
    public int Numb { get; set; } = default!;
    public string Description { get; set; } = default!;

    public string Theme { get; set; }

    public string StudentRep { get; set; }
    public string Author { get; set; }

    public DateTime CreateTime { get; set; }


}