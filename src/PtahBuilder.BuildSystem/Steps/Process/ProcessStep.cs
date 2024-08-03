using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Process
{
    public class ProcessStep<T> : IStep<T>
    {
        private readonly Action<Entity<T>> _act;

        public ProcessStep(Action<Entity<T>> act)
        {
            _act = act;
        }

        public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
        {
            foreach (var entity in entities)
            {
                _act(entity);
            }

            return Task.CompletedTask;
        }
    }
}
