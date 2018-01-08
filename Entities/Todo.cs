using System;
using System.Runtime.Serialization;

namespace Firelink.Todos.Entity
{
    public enum TodoStatus
    {
        [EnumMember(Value = "Abandoned")]
        Abandoned,
        [EnumMember(Value = "Blocked")]
        Blocked,
        [EnumMember(Value = "NotStarted")]
        NotStarted,
        [EnumMember(Value = "InProgress")]
        InProgress,
        [EnumMember(Value = "Completed")]
        Completed
    }

    public class TodoBag
    {
        public string Description {get;set;}
        public DateTime? Due { get; set; }
        public TodoStatus Status { get; set; } = TodoStatus.NotStarted;
    }

    public class Todo
    {
        public Guid Id { get; private set; }
        public string Description { get; private set; }
        public DateTime? Due { get; private set; }
        public DateTime? LastChanged { get; private set; } = DateTime.Today;
        public TodoStatus Status { get; private set; } = TodoStatus.NotStarted;

        private Todo()
        { }

        public Todo(string description, TodoStatus status = TodoStatus.NotStarted) : this()
        {
            Description = description;
            Status = status;
        }

        public Todo(string description, DateTime? due, TodoStatus status = TodoStatus.NotStarted) : this(description, status)
        {
            Due = due;
        }

        public void MarkAs(TodoStatus status, DateTime when)
        {
            Status = status;
            LastChanged = when;
        }

        public void MarkAs(TodoStatus status)
        {
            MarkAs(status, DateTime.Today);
        }

        public void UpdateFrom(TodoBag t)
        {
            if (Status != t.Status)
            {
                MarkAs(t.Status, DateTime.Today);
            }

            if (Due != t.Due)
            {
                Due = t.Due.HasValue ? t.Due : Due;
            }

            if (Description != t.Description)
            {
                Description = !string.IsNullOrWhiteSpace(t.Description) ? t.Description : Description;
            }
        }

        public static Todo CreateFrom(TodoBag t)
        {
            return new Todo(t.Description, t.Due, t.Status);
        }
    }
}
