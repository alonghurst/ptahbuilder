using System;

namespace PtahBuilder.TestBuilder.Types
{
    public class BaseTypeData
    {
        public string TypeName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string[] Description { get; set; } = Array.Empty<string>();
    }
}
