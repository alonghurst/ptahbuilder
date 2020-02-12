using System.Collections.Generic;
using System.Linq;
using BlueprintTech.CodeGeneration;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Syntax
{
    public class InstanceToTypeFactoryNamesFileWriter<T> : InstanceToTypeFactoryBase<T> 
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
            var entityId = MetadataResolver.GetEntityId(entity);
            var name = entityId.Substring(0, 1).ToUpper() + entityId.Substring(1, entityId.Length - 1);
            var value = Literals.String(entityId);

            return Fields.PublicConstField(name, typeof(string), value);
        }
    }
}
