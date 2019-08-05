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

    public interface IAggregateRoot
    {
    }

    public abstract class EntityBase
    {
        protected virtual object Actual => this;
        public int Id { get; private set; }

        protected virtual void UpdateId(int id)
        {
            if (Id == 0 && id > 0) { Id = id; }
        }

        public override int GetHashCode()
        {
            return (Actual.GetType().ToString() + Id).GetHashCode();
        }

        public static bool operator ==(EntityBase a, EntityBase b)
        {
            if (a is null && b is null) { return true; }
            if (a is null || b is null) { return false; }
            return a.Equals(b);
        }

        public static bool operator !=(EntityBase a, EntityBase b)
            =>  !(a == b);


        public override bool Equals(object obj)
        {
            if (!(obj is EntityBase other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            if (Actual.GetType() != other.Actual.GetType()) { return false; }
            if (Id == 0 || other.Id == 0) { return false; }
            return Id == other.Id;
        }
    }

    public abstract class ValueObjectBase
    {
        protected abstract IEnumerable<object> GetAtomicValues();

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType()) { return false; }

            ValueObjectBase other = (ValueObjectBase)obj;
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