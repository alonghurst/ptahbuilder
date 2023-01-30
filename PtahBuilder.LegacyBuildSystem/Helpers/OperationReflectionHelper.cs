using PtahBuilder.LegacyBuildSystem.Generators;
using PtahBuilder.LegacyBuildSystem.Metadata;
using PtahBuilder.Util.Helpers;

namespace PtahBuilder.LegacyBuildSystem.Helpers;

public static class OperationReflectionHelper
{
    public static Type FindBaseDataGeneratorType(Type forType)
    {
        return ReflectionHelper.FindDerivedTypeOrUseBaseType(forType, typeof(DataGenerator<>));
    }

    public static Type FindBaseDataMetadataResolverType(Type forType)
    {
        return ReflectionHelper.FindDerivedTypeOrUseBaseType(forType, typeof(BaseDataMetadataResolver<>));
    }

    public static Type FindOperationProviderType(Type forType)
    {
        return ReflectionHelper.FindDerivedTypeOrUseBaseType(forType, typeof(OperationProvider<>));
    }
    
    public static Type[] FindOperationTypes(Type forType)
    {
        var generatorBaseType = typeof(Operation<>).MakeGenericType(forType);

        return ReflectionHelper.GetLoadedTypesThatAreAssignableTo(generatorBaseType).ToArray();
    }
}