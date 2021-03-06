﻿using CodeSourceGenerator.Models.RenderModels;
using System.Collections.Generic;

namespace CodeSourceGenerator.Models.CodeGeneration.Collections
{
    public class FieldCollectionImpl : ItemCollectionImpl<Field>, IEnumerable<Field>
    {
        public FieldCollectionImpl(IEnumerable<Field> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Field item)
        {
            foreach (Attribute attribute in item.Attributes)
            {
                yield return attribute.Name;
                yield return attribute.FullName;
            }
        }

        protected override IEnumerable<string> GetItemFilter(Field item)
        {
            yield return item.Name;
            yield return item.FullName;
        }
    }
}