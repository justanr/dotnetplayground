using System;
using System.Linq.Expressions;
using Firelink.Todos.Entity;
using NSpecifications;


namespace Firelink.Todos.Specification
{

    using TodoSpec = ISpecification<Todo>;

    public static class TodoSpecifications
    {
        internal static TodoSpec ById(Guid id)
        {
            return Spec.For<Todo>(t => t.Id == id);
        }

        public static TodoSpec ByStatus(TodoStatus status)
        {
            return Spec.For<Todo>(t => t.Status == status);
        }

        public static readonly TodoSpec Workable = ByStatus(TodoStatus.NotStarted).Or(ByStatus(TodoStatus.InProgress));

        public static TodoSpec DueBefore(DateTime when)
        {
            return Spec.For<Todo>(t => t.Due <= when);
        }

        public static TodoSpec DueAfter(DateTime when)
        {
            return Spec.For<Todo>(t => t.Due >= when);
        }

        public static TodoSpec Due(DateTime when)
        {
            return Spec.For<Todo>(t => t.Due == when);
        }

        public static TodoSpec NoDueDate = Spec.For<Todo>(t => t.Due == null);
        public static TodoSpec HasDueDate = Spec.For<Todo>(t => t.Due != null);

        public static  TodoSpec Overdue() {
            return Workable.And(DueBefore(DateTime.Today));
        }

        public static TodoSpec NotOverdue()
        {
            return Workable.And(DueAfter(DateTime.Today).Or(NoDueDate));
        }

        public static TodoSpec InDateRange(DateTime begin, DateTime end)
        {
            return DueAfter(begin).And(DueBefore(end));
        }
    }
}
