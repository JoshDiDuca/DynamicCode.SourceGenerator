using CodeSourceGenerator.Models.RenderModels;
using System.Collections.Generic;

namespace CodeSourceGenerator.Models.CodeGeneration.Collections
{
    public class MethodCollectionImpl : ItemCollectionImpl<Method>, IEnumerable<Method>
    {
        public MethodCollectionImpl(IEnumerable<Method> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Method item)
        {
            foreach (Attribute attribute in item.Attributes)
            {
                yield return attribute.Name;
                yield return attribute.FullName;
            }
        }

        protected override IEnumerable<string> GetItemFilter(Method item)
        {
            yield return item.Name;
            yield return item.FullName;
        }
    }
}