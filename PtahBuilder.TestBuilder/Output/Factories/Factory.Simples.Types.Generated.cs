namespace PtahBuilder.TestBuilder.Output
{
    using PtahBuilder.TestBuilder.Types;

    public static partial class Factory
    {
        public static partial class Simples
        {
            public static partial class Types
            {
                static void Load()
                {
                    Add(new SimpleType { AnInt = 1, TypeName = "ST_0", Name = "First" });
                    Add(new SimpleType { AnInt = 3, AnEnum = SimpleEnum.Bravo, TypeName = "ST_1", Name = "Second", Description = new[] { "Possible to set", "An array", "Of Strings" } });
                    Add(new SimpleType { AnInt = 2, AnEnum = SimpleEnum.Charlie, TypeName = "ST_2", Name = "gnitset rof tcejbo drihT" });
                    Add(new SimpleType { AnInt = 4, ABoolean = true, TypeName = "ST_3", Name = "Delta" });
                }
            }
        }
    }
}