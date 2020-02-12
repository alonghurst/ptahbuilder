using System;
using System.Collections.Generic;
using System.Linq;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Operations
{
    public class FixPunctuationOperation<T> : IOperation<T> where T : TypeData
    {
        private readonly string _propertyName;

        public FixPunctuationOperation(string propertyName)
        {
            _propertyName = propertyName;
        }

        public Dictionary<T, MetadataCollection> Operate(Dictionary<T, MetadataCollection> entities)
        {
            var property = typeof(T).GetProperty(_propertyName);

            if (property == null)
            {
                throw new ArgumentNullException(_propertyName);
            }

            foreach (var entity in entities)
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

            return entities;
        }
    }
}
