using System;
using System.Collections.Generic;
using System.Linq;
using NSpecifications;
using Firelink.Todos.Entity;
using Firelink.Todos.Specification;
using Firelink.Todos.Reflection;

namespace Firelink.Todos.Specification
{
    using TodoSpec = ISpecification<Todo>;
    using RawTodoSpec = Dictionary<string, IEnumerable<string>>;

    public class TodoQueryParams
    {
        public List<TodoStatus> Include { get; set; } = new List<TodoStatus>();
        public List<TodoStatus> Exclude { get; set; } = new List<TodoStatus>();
        public bool? Overdue { get; set; }
        public DateTime? DueAfter { get; set; }
        public DateTime? DueBefore { get; set; }
        public DateTime? Due { get; set; }
        public bool? HasDueDate { get; set; }
    }

    public class TodoSpecificationBuilder : ISpecificationBuilder<Todo, TodoQueryParams>
    {
        public TodoSpecificationBuilder()
        { }

        public TodoSpec Build(TodoQueryParams request)
        {
            var specs = new List<TodoSpec>();

            specs.Add(BuildInclude(request.Include));
            specs.Add(BuildExclude(request.Exclude));
            specs.Add(BuildDue(
                request.Overdue, request.HasDueDate, exactly: request.Due, begin: request.DueAfter, end: request.DueBefore
            ));

            specs = specs.Where(x => x != null).ToList();

            if (specs.Count() < 1)
            {
                return Spec.All<Todo>();
            }

            return specs.Aggregate((spec, next) => spec.And(next));
        }

        private TodoSpec BuildDue(bool? overdue, bool? hasDueDate, DateTime? exactly, DateTime? begin, DateTime? end)
        {
            if (exactly.HasValue)
            {
                return BuildExactly(exactly.Value, overdue);
            }

            else if (!begin.HasValue && !end.HasValue)
            {
                return BuildOverdue(overdue, hasDueDate);
            }

            else if (!overdue.HasValue && !hasDueDate.HasValue)
            {
                return BuildDateRange(begin, end);
            }

            else
            {
                return BuildDateSelector(overdue, hasDueDate, begin, end);
            }
        }

        private TodoSpec BuildExactly(DateTime when, bool? overdue)
        {
            var spec = TodoSpecifications.Due(when);

            if (overdue.HasValue)
            {
                spec = spec.And(BuildOverdue(overdue.Value, hasDueDate: null));
            }

            return spec;
        }

        private TodoSpec BuildOverdue(bool? overdue, bool? hasDueDate)
        {
            if (overdue.HasValue && hasDueDate.HasValue)
            {
                return overdue.Value ? GetOverDue(true) : GetHasDueDate(hasDueDate.Value);
            }

            else if (!overdue.HasValue && hasDueDate.HasValue)
            {
                return GetHasDueDate(hasDueDate.Value);
            }

            else if (overdue.HasValue && !hasDueDate.HasValue)
            {
                return GetOverDue(overdue.Value);
            }

            else
            {
                return null;
            }
        }

        private TodoSpec BuildDateSelector(bool? overdue, bool? hasDueDate, DateTime? begin, DateTime? end)
        {
            var specification = BuildDateRange(begin, end);

            if (overdue.HasValue)
            {
                return specification.And(BuildOverdue(overdue.Value, hasDueDate: null));
            }

            return (hasDueDate.HasValue && hasDueDate.Value) ? specification : specification.Or(TodoSpecifications.NoDueDate);
        }

        private TodoSpec BuildDateRange(DateTime? begin, DateTime? end)
        {
            if (begin.HasValue && end.HasValue)
            {
                return TodoSpecifications.InDateRange(begin.Value, end.Value);
            }

            else if (begin.HasValue && !end.HasValue)
            {
                return TodoSpecifications.DueAfter(begin.Value);
            }

            else if (!begin.HasValue && end.HasValue)
            {
                return TodoSpecifications.DueBefore(end.Value);
            }
            else
            {
                return null;
            }
        }

        private TodoSpec GetOverDue(bool overdue)
        {
            return overdue ? TodoSpecifications.Overdue() : TodoSpecifications.NotOverdue();

        }

        private TodoSpec GetHasDueDate(bool hasDueDate)
        {
            return hasDueDate ? TodoSpecifications.HasDueDate : TodoSpecifications.NoDueDate;

        }

        private TodoSpec BuildInclude(IEnumerable<TodoStatus> statuses)
        {
            return BuildStatus(statuses)?.Aggregate((spec, next) => spec.Or(next));
        }

        private TodoSpec BuildExclude(IEnumerable<TodoStatus> statuses)
        {
            if (statuses.Count() == 0)
            {
                return null;
            }

            return BuildStatus(statuses)?.Select(s => (TodoSpec)s.Not()).Aggregate((s,n) => s.And(n));
        }

        private IEnumerable<TodoSpec> BuildStatus(IEnumerable<TodoStatus> statuses)
        {
            if (statuses.Count() == 0)
            {
                return null;
            }

            return statuses.Select(s => TodoSpecifications.ByStatus(s));
        }
    }
}
