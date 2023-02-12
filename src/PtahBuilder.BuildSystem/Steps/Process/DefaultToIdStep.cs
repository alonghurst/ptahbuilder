using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Process
{
    public class DefaultToIdStep<T> : IStep<T>
    {
        private readonly string _propertyName;

        public DefaultToIdStep(string propertyName)
        {
            _propertyName = propertyName;
        }

        public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
        {
            var property = typeof(T).GetProperty(_propertyName);

            if (property == null)
            {
                throw new InvalidOperationException($"Unable to find a property named \"{_propertyName}\" on type \"{typeof(T).Name}\"");
            }

            if (property.PropertyType != typeof(string))
            {
                throw new InvalidOperationException($"Property \"{_propertyName}\" on type \"{typeof(T).Name}\" is of type {property.PropertyType}");
            }

            if (!property.CanWrite)
            {
                throw new InvalidOperationException($"Property \"{_propertyName}\" on type \"{typeof(T).Name}\" is readonly");
            }

            foreach (var entity in entities)
            {
                if (string.IsNullOrWhiteSpace(property.GetValue(entity.Value)?.ToString()))
                {
                    property.SetValue(entity.Value, entity.Id);
                }
            }

            return Task.CompletedTask;
        }
    }
}
