﻿using Humanizer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Services;
using PtahBuilder.BuildSystem.Steps.Output.AdditionalTextOutput;
using PtahBuilder.Plugins.CodeGeneration.Syntax;

namespace PtahBuilder.Plugins.CodeGeneration.Steps;

public class EntityLiteralsConfig<T>
{
    public string OutputDirectory { get; set; } = string.Empty;
    public string Namespace { get; set; } = "Entities";
    public string Name { get; set; } = "Ids";
    public string FileType { get; set; } = ".generated.cs";
    public Func<Entity<T>, string> Accessor { get; set; } = x => x.Id;
}

public class EntityLiteralsOutputStep<T> : AdditionalOutputStepForAllEntities<T>
{
    private readonly IEntityMetadataService _entityMetadataService;
    private readonly EntityLiteralsConfig<T> _config;

    public EntityLiteralsOutputStep(IEntityMetadataService entityMetadataService, EntityLiteralsConfig<T> config) : base(config.OutputDirectory)
    {
        _entityMetadataService = entityMetadataService;
        _config = config;
    }

    protected override (string filename, string content) GenerateContent(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var filename = $"{_entityMetadataService.GetSimpleName(typeof(T))}{_config.Name}{_config.FileType}";

        var content = CreateCodeFile(entities).Definition();

        return (filename, content);
    }

    private CodeFile CreateCodeFile(IReadOnlyCollection<Entity<T>> entities)
    {
        return new()
        {
            DefinedInNamespace = _config.Namespace,
            ClassName = $"{_entityMetadataService.GetSimpleName(typeof(T))}{_config.Name}",
            AccessModifiers = new[]
            {
                SyntaxKind.PublicKeyword,
                SyntaxKind.StaticKeyword
            },
            Members = entities.Select(CreateConstant).ToArray
        };
    }

    private MemberDeclarationSyntax CreateConstant(Entity<T> entity)
    {
        var name = entity.Id.Pascalize().Replace("-", "_");
        
        var value = Literals.String(_config.Accessor(entity));

        return Fields.PublicConstField(name, typeof(string), value);
    }
}