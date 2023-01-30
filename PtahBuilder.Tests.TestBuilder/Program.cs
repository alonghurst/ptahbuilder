using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.Stages.Input;
using PtahBuilder.BuildSystem.Stages.Output;
using PtahBuilder.BuildSystem.Stages.Process;
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
            p.AddInputStage<JsonInputStage<Fruit>>();
            p.AddOutputStage<JsonOutputStage<Fruit>>();
        });

        x.AddPipeline<Recipe>(p =>
        {
            p.AddInputStage<JsonInputStage<Recipe>>();
            p.AddProcessStage<ValidateEntityReferenceStage<Recipe, Fruit>>((Recipe r) => r.ValidFruits);
            p.AddOutputStage<JsonOutputStage<Recipe>>();
        });
    })
    .Run();