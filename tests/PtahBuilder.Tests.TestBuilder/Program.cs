using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.Steps.Input;
using PtahBuilder.BuildSystem.Steps.Output;
using PtahBuilder.BuildSystem.Steps.Output.Code;
using PtahBuilder.BuildSystem.Steps.Process;
using PtahBuilder.Generators.ComponentModelDocumentation.Extensions;
using PtahBuilder.Tests.TestBuilder.Entities;
using PtahBuilder.Util.Helpers;
using Constants = PtahBuilder.Tests.TestBuilder.Constants;

await new BuilderFactory()
    .ConfigureFiles(f =>
    {
        f.Configure(PathHelper.GetRootPath("Data", args));

        f.AdditionalDirectories.Add(Constants.DirectoryKeys.CodeOutput, Path.Combine(f.OutputDirectory, "Code"));
    })
    .AddJsonConverterTypes(typeof(Program).Assembly)
    .AddCustomValueParser(typeof(PtahBuilder.Tests.TestBuilder.Entities.Range), v =>
    {
        try
        {
            return PtahBuilder.Tests.TestBuilder.Entities.Range.Parse(v.ToString()!);
        }
        catch
        {
            return null!;
        }
    })
    .AddComponentModelDocumentation()
    .ConfigureExecution(x =>
    {
        x.AddPipeline<Fruit>(p =>
        {
            p.AddInputStep<JsonInputStep<Fruit>>();
            p.AddOutputStep<JsonOutputStep<Fruit>>();
            p.AddOutputStep<EntityLiteralsOutputStep<Fruit>>(new EntityLiteralsConfig<Fruit>
            {
                OutputDirectory = x.Files.AdditionalDirectories[Constants.DirectoryKeys.CodeOutput]
            });
        });

        x.AddPipeline<Recipe>(p =>
        {
            p.AddInputStep<JsonInputStep<Recipe>>();
            p.AddProcessStep<ValidateEntityReferenceStep<Recipe, Fruit>>(nameof(Recipe.ValidFruits));
            p.AddOutputStep<JsonOutputStep<Recipe>>();
            p.AddOutputStep<EntityLiteralsOutputStep<Recipe>>(new EntityLiteralsConfig<Recipe>
            {
                OutputDirectory = x.Files.AdditionalDirectories[Constants.DirectoryKeys.CodeOutput]
            });
        });
        
        x.AddComponentModelDocumentationPipeline(p =>
        {
            p.AddTypesInheritedFrom(typeof(TypeData));
        });
    })
    .Run();