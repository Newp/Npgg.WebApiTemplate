using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiExample.Tests
{
    public class Counter
    {
        int Value;

        public int Increase() => Interlocked.Increment(ref Value);
    }
}
