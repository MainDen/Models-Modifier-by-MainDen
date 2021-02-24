using System;

namespace Modifiers_Core_by_MainDen.Modifiers
{
    public abstract class AbstractModifier
    {
        private static string description = "The modifier.";
        private static string[] argNames = new string[0];
        private static string[] argHints = new string[0];
        private static string[] argFormats = new string[0];
        private static string[] argDefaults = new string[0];
        
        public abstract string Name { get; }
        public virtual string Description { get; }
        public virtual string[] ArgNames => argNames;
        public virtual string[] ArgHints => argHints;
        public virtual string[] ArgFormats => argFormats;
        public virtual string[] ArgDefaults => argDefaults;
        public virtual string[] ArgStates { get; protected set; } = null;
        
        public abstract object ApplyTo(object model);

        public abstract bool CanBeAppliedTo(Type modelType);

        public abstract Type ResultType(Type modelType);

        protected bool ContainsNullArgStates()
        {
            string[] defaults = ArgDefaults;
            if (defaults is null)
                throw new Exception("Invalid class implementation.", new NullReferenceException("ArgDefaults return null value."));
            string[] states = ArgStates;
            if (states is null)
                return true;
            int dlen = defaults.Length;
            int slen = states.Length;
            if (dlen != slen)
                throw new Exception($"Expected {dlen} arguments (currently {slen}).");
            foreach (var state in states)
                if (state is null)
                    return true;
            return false;
        }
        public void Initialize()
        {
            string[] defaults = ArgDefaults;
            if (defaults is null)
                throw new Exception("Invalid class implementation.", new NullReferenceException("ArgDefaults return null value."));
            int len = defaults.Length;
            string[] states = new string[len];
            for (int i = 0; i < len; ++i)
                states[i] = defaults[i];
            ArgStates = states;
        }
        public AbstractModifier Modifier
        {
            get
            {
                if (this is InvokerModifier invoker)
                    return invoker.Modifier;
                try
                {
                    AbstractModifier modifier = (AbstractModifier)GetType().GetConstructor(new Type[0]).Invoke(new object[0]);
                    if (ContainsNullArgStates())
                        modifier.Initialize();
                    else
                    {
                        string[] sourceStates = ArgStates;
                        int len = sourceStates.Length;
                        string[] states = new string[len];
                        for (int i = 0; i < len; ++i)
                            states[i] = sourceStates[i];
                        modifier.ArgStates = states;
                    }
                    return modifier;
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid class implementation.", e);
                }
            }
        }
    }
}
