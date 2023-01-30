using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.Stages.Input;
using PtahBuilder.BuildSystem.Stages.Output;
using PtahBuilder.Tests.TestBuilder;
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
            p.AddStage<JsonInputStage<Fruit>>();
            p.AddStage<JsonOutputStage<Fruit>>();
        });
    })
    .Run();