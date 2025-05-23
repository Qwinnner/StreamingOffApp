using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using StreamingOffApp.Data;

namespace StreamingOffApp.Data
{
    public class StreamingContextFactory : IDesignTimeDbContextFactory<StreamingContext>
    {
        public StreamingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StreamingContext>();
            optionsBuilder.UseSqlServer("Server=PC_DAWID;Database=StreamingOffDB;Trusted_Connection=True;Encrypt=False");

            return new StreamingContext(optionsBuilder.Options);
        }
    }
}