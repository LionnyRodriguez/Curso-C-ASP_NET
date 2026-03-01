using TaskManager.Domain.Shared;

namespace TaskManager.Domain.Common
{
    public abstract class ValueObject : CheckableObject
    {
        public bool Equals(ValueObject? other)
        {
            if (other is null) return false;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }
        public abstract IEnumerable<object> GetEqualityComponents();
    }
}
