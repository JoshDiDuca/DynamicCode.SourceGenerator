using CodeSourceGenerator.Models.RenderModels;
using System.Collections.Generic;

namespace CodeSourceGenerator.Models.CodeGeneration.Collections
{
    public class EventCollectionImpl : ItemCollectionImpl<Event>, IEnumerable<Event>
    {
        public EventCollectionImpl(IEnumerable<Event> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Event item)
        {
            foreach (Attribute attribute in item.Attributes)
            {
                yield return attribute.Name;
                yield return attribute.FullName;
            }
        }

        protected override IEnumerable<string> GetItemFilter(Event item)
        {
            yield return item.Name;
            yield return item.FullName;
        }
    }
}