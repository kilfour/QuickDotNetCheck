namespace QuickDotNetCheck.ElaborateExample.Domain
{
    public interface IHaveAnId<TId>
    {
        TId Id { get; set; }
    }
}