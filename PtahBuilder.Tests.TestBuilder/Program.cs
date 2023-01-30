using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.Steps.Input;
using PtahBuilder.BuildSystem.Steps.Output;
using PtahBuilder.BuildSystem.Steps.Process;
using PtahBuilder.Tests.TestBuilder.Entities;
using PtahBuilder.Util.Helpers;

await new BuilderFactory()
    .ConfigureFiles(f =>
    {
        f.Configure(PathHelper.GetRootPath("Data", args));
    })
    .ConfigureExecution(x =>
    {
        x.AddPipeline<Fruit>(p =>
        {
            p.AddInputStage<JsonInputStep<Fruit>>();
            p.AddOutputStage<JsonOutputStep<Fruit>>();
        });

        x.AddPipeline<Recipe>(p =>
        {
            p.AddInputStage<JsonInputStep<Recipe>>();
            p.AddProcessStage<ValidateEntityReferenceStep<Recipe, Fruit>>((Recipe r) => r.ValidFruits);
            p.AddOutputStage<JsonOutputStep<Recipe>>();
        });
    })
    .Run();