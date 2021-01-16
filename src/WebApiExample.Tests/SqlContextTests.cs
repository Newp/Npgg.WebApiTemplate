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
        }

        [Fact]
        public void Test()
        {
        }
    }
}
