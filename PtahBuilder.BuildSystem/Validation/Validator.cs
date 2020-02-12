using System.Collections.Generic;
using System.Linq;
using Humanizer;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Generators;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Validation
{
    public abstract class Validator<T> : SecondaryGenerator<T>
    {
        protected Validator(Logger logger, BaseDataMetadataResolver<T> metadataResolver, PathResolver pathResolver, Dictionary<T, MetadataCollection> entities) : base(logger, metadataResolver, pathResolver, entities)
        {
        }

        [Generate]
        public void StringValidation()
        {
            var propertiesToValidate = StringPropertiesToValidate().ToArray();

            if (propertiesToValidate.Any())
            {
                Logger.LogSection($"{typeof(T).Name.Pluralize()} With Missing Values",
                    new StringParameterVerification<T>(f => MetadataResolver.GetEntityId(f), propertiesToValidate).Validate(Entities.Keys).Select(kvp => $"{kvp.Key} {kvp.Value.Name}"));
            }
        }

        protected virtual IEnumerable<string> StringPropertiesToValidate()
        {
            yield break;
        }
    }
}
