using System;

namespace Modifiers_by_MainDen.Modifiers
{
    public abstract class AbstractModifier
    {
        public abstract object ApplyTo(object model, params object[] args);

        public abstract bool CanBeAppliedTo(Type modelType);

        public abstract Type ResultType(Type modelType);

        public abstract string GetArgsInfo(object model);

        public abstract string Name { get; }
    }
}
