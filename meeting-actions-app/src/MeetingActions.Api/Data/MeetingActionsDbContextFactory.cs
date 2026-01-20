using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MeetingActions.Api.Data;

public class MeetingActionsDbContextFactory : IDesignTimeDbContextFactory<MeetingActionsDbContext>
{
    public MeetingActionsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MeetingActionsDbContext>();
        
        // Connection string para design-time (migraciones)
        optionsBuilder.UseSqlServer("Server=DESKTOP-AOVL974\\SQLEXPRESS;Database=MeetingActions;User Id=sa;Password=blasto01;TrustServerCertificate=True");

        return new MeetingActionsDbContext(optionsBuilder.Options);
    }
}
