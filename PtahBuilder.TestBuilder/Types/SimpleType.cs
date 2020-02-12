using System;
using System.Collections.Generic;
using System.Text;
using PtahBuilder.BuildSystem;

namespace PtahBuilder.TestBuilder.Types
{
    public class SimpleType : BaseTypeData
    {
        public int AnInt { get; set; }
        public bool ABoolean { get; set; }
        public SimpleEnum AnEnum { get; set; }
    }
}
