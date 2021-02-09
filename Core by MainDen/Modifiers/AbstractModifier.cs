using Core_by_MainDen.Models;

namespace Core_by_MainDen.Modifiers
{
    public abstract class AbstractModifier
    {
        public abstract AbstractModel ApplyTo(AbstractModel model, params object[] args);

        public abstract bool CanBeAppliedTo(AbstractModel model);

        public abstract string GetArgsInfo(AbstractModel model);

        public abstract string Name { get; }
    }
}
