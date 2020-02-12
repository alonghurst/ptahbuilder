using System.Collections.Generic;
using System.Linq;
using BlueprintTech.CodeGeneration;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Syntax
{
    public class InstanceToTypeFactoryNamesFileWriter<T> : InstanceToTypeFactoryBase<T> where T : TypeData
    {
        protected override string SubClassName => "Names";

        public InstanceToTypeFactoryNamesFileWriter(Logger logger, BaseDataMetadataResolver<T> metadataResolver) : base(logger, metadataResolver)
        {
        }

        protected override IEnumerable<MemberDeclarationSyntax> Members(T[] instancesAr)
        {
            var grouped = instancesAr.GroupBy(MetadataResolver.GetEntityCategory);

            foreach (var group in grouped)
            {
                yield return Constructs.Class(string.IsNullOrEmpty(group.Key) ? "All" : group.Key, new[] { SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword }, () => SyntaxFactory.List(@group.Select(CreateConstant)));
            }
        }

        private MemberDeclarationSyntax CreateConstant(T entity)
        {
            var name = entity.TypeName.Substring(0, 1).ToUpper() + entity.TypeName.Substring(1, entity.TypeName.Length - 1);
            var value = Literals.String(entity.TypeName);

            return Fields.PublicConstField(name, typeof(string), value);
        }
    }
}
