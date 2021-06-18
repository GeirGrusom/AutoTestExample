using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebProShop.Data.Migration
{
    public interface IDatabaseMigration
    {
        Task<List<string>> Migrate(CancellationToken cancel);
    }
}