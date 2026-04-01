namespace Choam.Domain.Entities;

public class AppUser
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FirstLogin { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
