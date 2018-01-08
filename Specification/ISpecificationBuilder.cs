using NSpecifications;
using System.Collections.Generic;

namespace Firelink.Todos.Specification
{
    public interface ISpecificationBuilder<T,U>
    {
        ISpecification<T> Build(U parts);
    }
}
