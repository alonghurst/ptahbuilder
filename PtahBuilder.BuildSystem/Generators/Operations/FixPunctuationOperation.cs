using System;
using System.Linq;
using PtahBuilder.BuildSystem.Generators.Context;

namespace PtahBuilder.BuildSystem.Generators.Operations
{
    public class FixPunctuationOperation<T> : Operation<T> 
    {
        private readonly string _propertyName;

        public FixPunctuationOperation(string propertyName, IOperationContext<T> context) : base(context)
        {
            _propertyName = propertyName;
        }

        [Operate]
        public void Operate()
        {
            var property = typeof(T).GetProperty(_propertyName);

            if (property == null)
            {
                throw new ArgumentNullException(_propertyName);
            }

            foreach (var entity in Entities)
            {
                if (property.GetValue(entity.Key) is string[] text)
                {
                    text = text.Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();

                    if (text.Length == 0)
                    {
                        text = null;
                    }
                    else
                    {
                        for (int i = 0; i < text.Length; i++)
                        {
                            var description = text[i];
                            if (!description.EndsWith(".") && !description.EndsWith("?") && !description.EndsWith("!"))
                            {
                                text[i] = $"{description}.";
                            }
                        }
                    }

                    property.SetValue(entity.Key, text);
                }
            }
        }
    }
}
