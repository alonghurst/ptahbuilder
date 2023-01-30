using PtahBuilder.BuildSystem.Execution;

namespace PtahBuilder.BuildSystem.Config
{
    public abstract class PipelineConfig
    {
        public string Name { get; }

        public List<Type> Stages { get; } = new();

        public PipelineConfig(string name)
        {
            Name = name;
        }

        public PipelineConfig Configure(Action<PipelineConfig> configure)
        {
            return this;
        }

        public PipelineConfig AddStage(Type stageType)
        {
            Stages.Add(stageType);

            return this;
        }
    }

    public class PipelineConfig<T> : PipelineConfig
    {
        public Func<T, string> GetId { get; set; }

        public PipelineConfig(string name) : base(name)
        {
            GetId = CreateDefaultGetId();
        }

        private Func<T, string> CreateDefaultGetId()
        {
            var properties = typeof(T).GetProperties();

            var entityId = properties.FirstOrDefault(x => x.Name == $"{typeof(T).Name}Id");

            if (entityId != null)
            {
                return x => entityId.GetValue(x)?.ToString() ?? throw new InvalidOperationException();
            }

            var id = properties.FirstOrDefault(x => x.Name == "Id");

            if (id != null)
            {
                return x => id.GetValue(x)?.ToString() ?? throw new InvalidOperationException();
            }

            var typeName = properties.FirstOrDefault(x => x.Name == "TypeName");

            if (typeName != null)
            {
                return x => typeName.GetValue(x)?.ToString() ?? throw new InvalidOperationException();
            }

            var name = properties.FirstOrDefault(x => x.Name == "Name");

            if (name != null)
            {
                return x => name.GetValue(x)?.ToString() ?? throw new InvalidOperationException();
            }

            return _ => Guid.NewGuid().ToString();
        }

        public PipelineConfig AddStage<TS>() where TS : IStage<T>
        {
            base.AddStage(typeof(TS));

            return this;
        }

        public PipelineConfig Configure(Action<PipelineConfig<T>> configure)
        {
            return this;
        }
    }
}
