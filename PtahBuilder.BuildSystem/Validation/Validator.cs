using System.Collections.Generic;
using System.Linq;
using Humanizer;
using PtahBuilder.BuildSystem.Generators;
using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Generators.Operations;

namespace PtahBuilder.BuildSystem.Validation
{
    public abstract class Validator<T> : Operation<T>
    {
        public override int Priority => int.MaxValue / 2;

        protected Validator(IOperationContext<T> context) : base(context)
        {
        }

        [Operate]
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
