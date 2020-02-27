using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Abc.Data.Quantity;
using Abc.Domain.Common;

namespace Abc.Domain.Quantity
{
    public class Measure : Entity<MeasureData>
    {
        public Measure(MeasureData data) : base(data)
        {

        }
    }
}
