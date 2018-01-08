using System;
using System.Collections.Generic;
using System.Linq;

namespace Firelink.Todos.Entity
{
    public class TodoList
    {
        public readonly Guid Id = Guid.NewGuid();
        public string Name {get; private set;}
        private readonly List<Todo> _todos = new List<Todo>();
        public IEnumerable<Todo> Todos => _todos.AsReadOnly();

        private TodoList()
        {}

        public TodoList(string name) : this()
        {
            Name = name;
        }

        public TodoList(string name, IEnumerable<Todo> todos) : this(name)
        {
            _todos = new List<Todo>(todos);
        }

        public void AddTodo(Todo t)
        {
            _todos.Add(t);
        }

        public void RemoveTodo(Todo t)
        {
            _todos.Add(t);
        }
    }
}
