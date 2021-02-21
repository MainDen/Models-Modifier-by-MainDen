using System;
using System.Reflection;

namespace Modifiers_by_MainDen.Modifiers
{
    public class InvokerModifier : AbstractModifier
    {
        private object modifier;
        private Type type;
        private string[] argStates = new string[0];

        public InvokerModifier()
        {
            modifier = null;
            type = null;
        }
        public InvokerModifier(object modifier)
        {
            this.modifier = modifier;
            type = modifier?.GetType();
        }

        public override string Name
        {
            get
            {
                if (modifier is null)
                    return "Empty";
                try
                {
                    const string str = nameof(Name);
                    string res = (string)type.InvokeMember(str, BindingFlags.GetProperty, null, modifier, null);
                    if (res is null)
                        throw new NullReferenceException(str + " must not be null.");
                    return res;
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid class implementation.", e);
                }
            }
        }
        public override string Description
        {
            get
            {
                if (modifier is null)
                    return "Returns the input model.";
                try
                {
                    const string str = nameof(Description);
                    string res = (string)type.InvokeMember(str, BindingFlags.GetProperty, null, modifier, null);
                    if (res is null)
                        throw new NullReferenceException(str + " must not be null.");
                    return res;
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid class implementation.", e);
                }
            }
        }
        public override string[] ArgNames
        {
            get
            {
                if (modifier is null)
                    return new string[0];
                try
                {
                    const string str = nameof(ArgNames);
                    string[] res = (string[])type.InvokeMember(str, BindingFlags.GetProperty, null, modifier, null);
                    if (res is null)
                        throw new NullReferenceException(str + " must not be null.");
                    return res;
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid class implementation.", e);
                }
            }
        }
        public override string[] ArgHints
        {
            get
            {
                if (modifier is null)
                    return new string[0];
                try
                {
                    const string str = nameof(ArgHints);
                    string[] res = (string[])type.InvokeMember(str, BindingFlags.GetProperty, null, modifier, null);
                    if (res is null)
                        throw new NullReferenceException(str + " must not be null.");
                    return res;
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid class implementation.", e);
                }
            }
        }
        public override string[] ArgFormats
        {
            get
            {
                if (modifier is null)
                    return new string[0];
                try
                {
                    const string str = nameof(ArgFormats);
                    string[] res = (string[])type.InvokeMember(str, BindingFlags.GetProperty, null, modifier, null);
                    if (res is null)
                        throw new NullReferenceException(str + " must not be null.");
                    return res;
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid class implementation.", e);
                }
            }
        }
        public override string[] ArgDefaults
        {
            get
            {
                if (modifier is null)
                    return new string[0];
                try
                {
                    const string str = nameof(ArgDefaults);
                    string[] res = (string[])type.InvokeMember(str, BindingFlags.GetProperty, null, modifier, null);
                    if (res is null)
                        throw new NullReferenceException(str + " must not be null.");
                    return res;
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid class implementation.", e);
                }
            }
        }
        public override string[] ArgStates
        {
            get
            {
                if (modifier is null)
                    return argStates;
                try
                {
                    const string str = nameof(ArgStates);
                    string[] res = (string[])type.InvokeMember(str, BindingFlags.GetProperty, null, modifier, null);
                    return res;
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid class implementation.", e);
                }
            }
            protected set
            {
                if (modifier is null)
                {
                    argStates = value;
                    return;
                }
                try
                {
                    const string str = nameof(ArgStates);
                    PropertyInfo setter = type.GetProperty(str);
                    setter.SetValue(modifier, value);
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid class implementation.", e);
                }
            }
        }
        public override object ApplyTo(object model)
        {
            if (modifier is null)
                return model;
            try
            {
                const string str = nameof(ApplyTo);
                object res = type.InvokeMember(str, BindingFlags.InvokeMethod, null, modifier, new object[] { model });
                if (res is null)
                    throw new NullReferenceException(str + " must not return null.");
                return res;
            }
            catch (Exception e)
            {
                throw new Exception("Invalid class implementation.", e);
            }
        }
        public override bool CanBeAppliedTo(Type modelType)
        {
            if (modifier is null)
                return !(modelType is null);
            try
            {
                const string str = nameof(CanBeAppliedTo);
                bool res = (bool)type.InvokeMember(str, BindingFlags.InvokeMethod, null, modifier, new object[] { modelType });
                return res;
            }
            catch (Exception e)
            {
                throw new Exception("Invalid class implementation.", e);
            }
        }
        public override Type ResultType(Type modelType)
        {
            if (modifier is null)
                return modelType;
            try
            {
                const string str = nameof(ResultType);
                Type res = (Type)type.InvokeMember(str, BindingFlags.InvokeMethod, null, modifier, new object[] { modelType });
                if (res is null)
                    throw new NullReferenceException(str + " must not return null."); 
                return res;
            }
            catch (Exception e)
            {
                throw new Exception("Invalid class implementation.", e);
            }
        }

        public new AbstractModifier Modifier
        {
            get
            {
                if (modifier is null)
                    return new InvokerModifier();
                try
                {
                    InvokerModifier modifier = new InvokerModifier(type.GetConstructor(new Type[0]).Invoke(new object[0]));
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
