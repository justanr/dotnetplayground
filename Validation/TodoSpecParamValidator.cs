using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Firelink.Todos.Specification;
using Firelink.Todos.Entity;

namespace Firelink.Todos.Validator
{

    public class TodoQueryValidator : FluentValidation.AbstractValidator<TodoQueryParams>
    {
        public TodoQueryValidator()
        {
            RuleForEach(q => q.Include)
                .Must((query, include) => !query.Exclude.Contains(include))
                .WithMessage("include and exclude cannot overlap");

            RuleFor(q => q.Overdue).Must(o => o == null || o == false)
                .When(q => q.HasDueDate == false)
                .WithMessage("overdue true and hasduedate false are incompatible");

            RuleFor(q => q.HasDueDate)
                .Equal(true)
                .When(q => q.DueAfter != null || q.DueBefore != null)
                .WithMessage("cannot specify both hasduedate false and date range")
                .Equal(true)
                .When(q => q.Due != null)
                .WithMessage("cannot specify hasduedate false and specific date");

            RuleFor(q => q.Due)
                .Null()
                .When(q => q.DueAfter != null || q.DueBefore != null)
                .WithMessage("cannot specify specific date and date range");

            RuleFor(q => q.Exclude)
                .Must(e => !e.Contains(TodoStatus.NotStarted) && !e.Contains(TodoStatus.InProgress))
                .When(q => q.Overdue == true)
                .WithMessage("excluding NotStart and InProgress negate overdue");
        }
    }
}
