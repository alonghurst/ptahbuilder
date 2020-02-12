using System.Collections.Generic;
using System.Linq;
using Humanizer;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Generators;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Validation
{
    public abstract class Validator<T> : SecondaryGenerator<T> where T : TypeData
    {
        protected Validator(Logger logger, PathResolver pathResolver, Dictionary<T, MetadataCollection> entities) : base(logger, pathResolver, entities)
        {
        }

        [Generate]
        public void StringValidation()
        {
            var propertiesToValidate = StringPropertiesToValidate().ToArray();

            if (propertiesToValidate.Any())
            {
                Logger.LogSection($"{typeof(T).Name.Pluralize()} With Missing Values",
                    new StringParameterVerification<T>(f => f.TypeName, propertiesToValidate).Validate(Entities.Keys).Select(kvp => $"{kvp.Key} {kvp.Value.Name}"));
            }
        }

        protected void Verify<TTarget>(TTarget[] targets, T entity, string forProperty, params string[] toFinds) where TTarget : TypeData
        {
            foreach (var toFind in toFinds)
            {
                if (targets.FirstOrDefault(r => r.TypeName == toFind) == null)
                {
                    Logger.LogSection($"{typeof(TTarget).Name} Not Found", $"{typeof(T).Name} {entity.Name}.{forProperty} = {toFind}");
                }
            }
        }

        protected void Verify<TTarget>(TTarget target, T entity, string forProperty, string toFind) where TTarget : TypeData
        {
            if (target?.TypeName != toFind)
                Logger.LogSection($"{typeof(TTarget).Name}. Not Found", $"{typeof(T).Name} {entity.Name}.{forProperty} = {toFind}");
        }

        protected virtual IEnumerable<string> StringPropertiesToValidate()
        {
            yield return nameof(TypeData.TypeName);
            yield return nameof(TypeData.Name);
            yield return nameof(TypeData.Description);
        }
    }
}
