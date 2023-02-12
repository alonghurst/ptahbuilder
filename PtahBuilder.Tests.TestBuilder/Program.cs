using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.Steps.Input;
using PtahBuilder.BuildSystem.Steps.Output;
using PtahBuilder.BuildSystem.Steps.Process;
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
    .ConfigureExecution(x =>
    {
        x.AddPipeline<Fruit>(p =>
        {
            p.AddInputStep<JsonInputStep<Fruit>>();
            p.AddOutputStep<JsonOutputStep<Fruit>>();
        });

        x.AddPipeline<Recipe>(p =>
        {
            p.AddInputStep<JsonInputStep<Recipe>>();
            p.AddProcessStep<ValidateEntityReferenceStep<Recipe, Fruit>>((Recipe r) => r.ValidFruits);
            p.AddOutputStep<JsonOutputStep<Recipe>>();
        });
    })
    .Run();