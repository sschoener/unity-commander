using System.Collections.Generic;

namespace Pasta.Finder
{
    public interface IConsumer<T>
    {
        /// <summary>
        /// Pass the given values to the consumer.
        /// </summary>
        /// <param name="values"></param>
        void Consume(IEnumerable<T> values);
        
        /// <summary>
        /// Whether or not this consumer is still valid.
        /// </summary>
        bool IsValid { get; }
    }
}