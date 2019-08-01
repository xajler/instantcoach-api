using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }

    public abstract class AggregateRoot : Entity
    {
    }

    public abstract class Entity : IEquatable<Entity>
    {
        protected virtual object Actual => this;
        public int Id { get; private set; }

        protected void UpdateId(int id)
        {
            if (Id == 0 && id > 0) { Id = id; }
        }

        public override int GetHashCode()
        {
            return (Actual.GetType().ToString() + Id).GetHashCode();
        }

        public bool Equals(Entity other)
        {
            if (other is null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            if (Actual.GetType() != other.Actual.GetType()) { return false; }
            if (Id == 0 || other.Id == 0) { return false; }
            return Id == other.Id;
        }
    }

    public abstract class ValueObject
    {
        protected abstract IEnumerable<object> GetAtomicValues();

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType()) { return false; }

            ValueObject other = (ValueObject)obj;
            IEnumerator<object> thisValues = GetAtomicValues().GetEnumerator();
            IEnumerator<object> otherValues = other.GetAtomicValues().GetEnumerator();
            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (thisValues.Current != null &&
                    !thisValues.Current.Equals(otherValues.Current))
                {
                    return false;
                }
            }
            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        public override int GetHashCode()
        {
            return GetAtomicValues()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
        }
    }
}