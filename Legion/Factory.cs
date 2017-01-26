using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace Skogsaas.Legion
{
    internal class Factory
    {
        private Dictionary<Type /* Interface */, Type /* Generated */> types;

        public delegate void TypeRegisteredHandler(Type type, Type generated);

        public event TypeRegisteredHandler OnTypeRegistered;

        public Factory()
        {
            this.types = new Dictionary<Type, Type>();
        }

        public void RegisterType(Type type)
        {
            if (!this.types.ContainsKey(type))
            {
                registerType(type, generateType(type));
            }
        }

        public Type FindType(Type type)
        {
            if(this.types.ContainsKey(type))
            {
                return this.types[type];
            }

            return null;
        }

        public Type FindType(string fullname)
        {
            foreach (KeyValuePair<Type, Type> pair in this.types)
            {
                if (pair.Key.FullName == fullname)
                {
                    return pair.Value;
                }
            }

            return null;
        }

        private void registerType(Type type, Type generated)
        {
            if (!this.types.ContainsKey(type))
            {
                this.types[type] = generated;

                this.OnTypeRegistered?.Invoke(type, generated);
            }
        }

        #region Class generation

        private Type generateType(Type type)
        {
            AssemblyName assemblyName = new AssemblyName("DynamicTypesAssembly");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, true);

            Type parent = null;
            Type baseType = null;

			if (typeof(IObject).IsAssignableFrom(type))
			{
                parent = typeof(ObjectBase);
                baseType = typeof(IObject);
            }
			else if (typeof(IStruct).IsAssignableFrom(type))
			{
                parent = typeof(StructBase);
                baseType = typeof(IStruct);
            }
            else if (typeof(IEvent).IsAssignableFrom(type))
            {
                parent = typeof(EventBase);
                baseType = typeof(IEvent);
            }
            else 
			{
				throw new NotSupportedException();
			}

            // Get the first interface this type implements
            Type[] interfaces = type.GetInterfaces();

            if(interfaces.Length > 0 && interfaces[0] != baseType)
            {
                RegisterType(interfaces[0]); // Makes sure the type is registered if it's not already.
                parent = FindType(interfaces[0]);
            }

            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                type.Name, // Name of class
                TypeAttributes.Public | TypeAttributes.BeforeFieldInit, // Attributes
                parent, // Parent
                new Type[] { type } // Interfaces
                );

            generateConstructorNoParam(parent, typeBuilder);

            if(baseType != typeof(IStruct))
            {
                generateConstructorIdParam(parent, typeBuilder);
            }

            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                generateProperty(property, typeBuilder, baseType);
            }

            return typeBuilder.CreateType();
        }

        private void generateConstructorNoParam(Type parent, TypeBuilder typeBuilder)
        {
            ConstructorInfo baseConstructor = parent.GetConstructor(new Type[] { });
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                null
                );

            // Generate code to call base constructor passing string
            ILGenerator ctorIL = constructorBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, baseConstructor);
            ctorIL.Emit(OpCodes.Ret);
        }

        private void generateConstructorIdParam(Type parent, TypeBuilder typeBuilder)
        {
            ConstructorInfo baseConstructor = parent.GetConstructor(new Type[] { typeof(string) });
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[] { typeof(string) }
                );

            // Generate code to call base constructor passing string
            ILGenerator ctorIL = constructorBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Call, baseConstructor);
            ctorIL.Emit(OpCodes.Ret);
        }

        private void generateProperty(PropertyInfo property, TypeBuilder typeBuilder, Type baseType)
        {
            if(property.PropertyType.IsGenericType)
            {
                foreach(Type t in property.PropertyType.GetGenericArguments())
                {
                    if(typeof(IStruct).IsAssignableFrom(t))
                    {
                        RegisterType(t);
                    }
                }
            }

            FieldBuilder fieldBuilder = typeBuilder.DefineField(
                    "_" + property.Name,
                    property.PropertyType,
                    FieldAttributes.Private
                    );

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
                property.Name,
                PropertyAttributes.HasDefault,
                property.PropertyType,
                null
                );

            MethodAttributes getsetAttributes =
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig |
                MethodAttributes.Virtual |
                MethodAttributes.NewSlot |
                MethodAttributes.Final;

            // Getter
            MethodBuilder methodGetBuilder = typeBuilder.DefineMethod(
            property.GetMethod.Name,
            getsetAttributes,
            property.PropertyType,
            Type.EmptyTypes
            );

            ILGenerator ilGetGenerator = methodGetBuilder.GetILGenerator();
            ilGetGenerator.Emit(OpCodes.Ldarg_0);
            ilGetGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            ilGetGenerator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodGetBuilder, property.GetMethod);

            // Setter
            MethodBuilder methodSetBuilder = typeBuilder.DefineMethod(
            property.SetMethod.Name,
            getsetAttributes,
            null,
            new Type[] { property.PropertyType }
            );

            ILGenerator ilSetGenerator = methodSetBuilder.GetILGenerator();
            ilSetGenerator.Emit(OpCodes.Ldarg_0); // "this"
            ilSetGenerator.Emit(OpCodes.Ldarg_1);
            ilSetGenerator.Emit(OpCodes.Stfld, fieldBuilder);

            if(baseType == typeof(IStruct) || baseType == typeof(IObject))
            {
                // Now call the NotifyPropertyChanged-function in the base class.
                ilSetGenerator.Emit(OpCodes.Ldarg_0); // "this"
                ilSetGenerator.Emit(OpCodes.Ldstr, property.Name);
                ilSetGenerator.EmitCall(
                    OpCodes.Call,
                    typeof(StructBase).GetMethod("NotifyPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic),
                    new Type[] { typeof(string) }
                    );
                ilSetGenerator.Emit(OpCodes.Nop);

                // If this property is a object inheriting from INotifyPropertyChanged,
                // Subscribe to changes downwards.
                if (typeof(INotifyPropertyChanged).IsAssignableFrom(property.PropertyType))
                {
                    ilSetGenerator.Emit(OpCodes.Ldarg_0);
                    ilSetGenerator.Emit(OpCodes.Ldarg_0);
                    ilSetGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                    ilSetGenerator.Emit(OpCodes.Ldstr, property.Name);
                    ilSetGenerator.EmitCall(
                        OpCodes.Call,
                        typeof(StructBase).GetMethod("ListenSubstructureChanged", BindingFlags.Instance | BindingFlags.NonPublic),
                        new Type[] { typeof(INotifyPropertyChanged), typeof(string) }
                        );
                    ilSetGenerator.Emit(OpCodes.Nop);
                }
            }

            ilSetGenerator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodSetBuilder, property.SetMethod);

            propertyBuilder.SetGetMethod(methodGetBuilder);
            propertyBuilder.SetSetMethod(methodSetBuilder);
        }

        #endregion
    }
}
