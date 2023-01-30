using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.Steps.Input;
using PtahBuilder.BuildSystem.Steps.Output;
using PtahBuilder.BuildSystem.Steps.Process;
using PtahBuilder.Tests.TestBuilder.Entities;
using PtahBuilder.Tests.TestBuilder.Entities.Conditions;
using PtahBuilder.Tests.TestBuilder.Entities.Dice;
using PtahBuilder.Util.Helpers;

await new BuilderFactory()
    .ConfigureFiles(f =>
    {
        f.Configure(PathHelper.GetRootPath("Data", args));
    })
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
    .AddCustomValueParser(typeof(IDiceValue), v =>
    {
        try
        {
            return DiceParser.Parse(v.ToString()!);
        }
        catch
        {
            return null!;
        }
    })
    .AddCustomValueParser(typeof(ICondition), v =>
    {
        try
        {
            return ConditionParser.Parse(v.ToString()!.Replace("\"", string.Empty));
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
            p.AddInputStage<JsonInputStep<Fruit>>();
            p.AddOutputStage<JsonOutputStep<Fruit>>();
        });

        x.AddPipeline<Recipe>(p =>
        {
            p.AddInputStage<JsonInputStep<Recipe>>();
            p.AddProcessStage<ValidateEntityReferenceStep<Recipe, Fruit>>((Recipe r) => r.ValidFruits);
            p.AddOutputStage<JsonOutputStep<Recipe>>();
        });

        x.AddPipeline<CreatureType>(p =>
        {
            p.AddInputStage<YamlInputStep<CreatureType>>();
            p.AddOutputStage<JsonOutputStep<CreatureType>>();
        });
    })
    .Run();