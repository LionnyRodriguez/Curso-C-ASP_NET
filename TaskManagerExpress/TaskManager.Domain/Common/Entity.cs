using TaskManager.Domain.Shared;

namespace TaskManager.Domain.Common
{
    public abstract class Entity : CheckableObject
    {
        public Guid Id { get; }
        protected Entity() { }
        protected Entity(Guid id)
        {
            Id = id;
        }
    }
}
