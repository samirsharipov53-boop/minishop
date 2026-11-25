namespace Domain.Models;

public class Stock_movements
{
    public int Id {get;set;}
    public int ProductId {get;set;}
    public int change_qty {get;set;}
    public DateTime moved_at {get;set;}
}
