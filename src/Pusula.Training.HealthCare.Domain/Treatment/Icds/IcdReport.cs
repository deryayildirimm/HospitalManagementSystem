using System.Diagnostics.CodeAnalysis;
using Volo.Abp;

namespace Pusula.Training.HealthCare.Treatment.Icds;

public class IcdReport
{
    [NotNull]
    public string CodeNumber { get; private set; } = null!;
    [NotNull]
    public string Detail { get; private set; } = null!;
    [NotNull]
    public int Quantity { get; private set; }

    protected IcdReport()
    {
        CodeNumber = string.Empty;
        Detail = string.Empty;
        Quantity = 0;
    }
    
    public IcdReport(string codeNumber, string detail, int quantity)
    {
        SetCodeNumber(codeNumber);
        SetDetail(detail);
        SetQuantity(quantity);
    }

    public void SetCodeNumber(string codeNumber)
    {
        Check.NotNullOrWhiteSpace(codeNumber, nameof(codeNumber), IcdConsts.CodeNumberMaxLength, IcdConsts.CodeNumberMinLength);
        CodeNumber = codeNumber.ToUpper();
    }

    public void SetDetail(string detail)
    {
        Check.NotNullOrWhiteSpace(detail, nameof(detail), IcdConsts.DetailMaxLength, IcdConsts.DetailMinLength);
        Detail = detail;
    }

    public void SetQuantity(int quantity)
    {
        Check.Range(quantity, nameof(quantity), 0);
        Quantity = quantity;
    }
}