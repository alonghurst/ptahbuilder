﻿using System;
using System.Collections.Generic;
using System.Linq;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Generators;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Operations
{
    public class FixPunctuationOperation<T> : Operation<T> 
    {
        private readonly string _propertyName;

        public FixPunctuationOperation(string propertyName, Logger logger, PathResolver pathResolver, BaseDataMetadataResolver<T> metadataResolver, Dictionary<T, MetadataCollection> entities) : base(logger, pathResolver, metadataResolver, entities)
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
