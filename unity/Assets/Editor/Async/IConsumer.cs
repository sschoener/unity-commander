using System.Collections.Generic;

namespace Pasta.Finder
{
    public interface IConsumer<T>
    {
        void Consume(IEnumerable<T> value);
        
        /// <summary>
        /// Whether or not this consumer is still valid.
        /// </summary>
        bool IsValid { get; }
    }
}