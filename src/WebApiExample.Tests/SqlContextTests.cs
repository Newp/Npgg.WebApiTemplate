using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebApiExample.Tests
{
    public class SqlContextTests : BaseFixture
    {
        readonly SqlContext context;
        public SqlContextTests()
        {
            this.context = base.GetService<SqlContext>();

            this.context.Database.EnsureCreated();
        }

        [Fact]
        public async Task Test()
        {
            var kvp = new KeyValue()
            {
                Key = Guid.NewGuid().ToString(),
                Value = "abbaasf"
            };

            this.context.KeyValues?.Add(kvp);
            await this.context.SaveChangesAsync();

        }
    }
}
