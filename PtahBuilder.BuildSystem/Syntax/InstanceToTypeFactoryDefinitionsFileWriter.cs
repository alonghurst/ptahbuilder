using PtahBuilder.CodeGeneration;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Syntax;

public class InstanceToTypeFactoryDefinitionsFileWriter<T> : InstanceToTypeFactoryBase<T> 
{
    protected override string SubClassName=>"Types";

    public InstanceToTypeFactoryDefinitionsFileWriter(Logger logger, BaseDataMetadataResolver<T> metadataResolver) : base(logger, metadataResolver)
    {
    }

    protected override IEnumerable<MemberDeclarationSyntax> Members(T[] instancesAr)
    {
        yield return Methods.Header("Load", Types.Void)
            .WithModifiers(SyntaxFactory.TokenList(Tokens.Static))
            .WithBody(Generate(instancesAr).AsBlock());
    }

    private IEnumerable<StatementSyntax> Generate(IEnumerable<T> instances)
    {
        foreach (var instance in instances)
        {
            var instantiation = Mapper.InstanceToSyntax(instance);
            yield return Invocations.InvokeOnLocalMethod("Add", instantiation.AsArgument()).AsStatement();
        }
    }
}