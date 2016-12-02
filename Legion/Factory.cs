using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace Legion
{
    internal class Factory
    {
        #region Singelton

        static Factory instance;
        private static Factory Instance
        {
            get
            {
                if (Factory.instance == null)
                {
                    Factory.instance = new Factory();
                }

                return Factory.instance;
            }
        }

        #endregion

        public static void RegisterType(Type type)
        {
            Factory.Instance.registerType(type);
        }

        public static Type FindType(Type type)
        {
            return Factory.Instance.findType(type);
        }

        public static Type FindType(string fullname)
        {
            return Factory.Instance.findType(fullname);
        }

        public static string GenerateId()
        {
            return Factory.Instance.generateId();
        }

        #region Implementation

        private Dictionary<Type /* Interface */, Type /* Generated */> types;

        private Factory()
        {
            this.types = new Dictionary<Type, Type>();
        }

        private void registerType(Type type)
        {
            if (!this.types.ContainsKey(type))
            {
                this.types[type] = generateType(type);
            }
        }

        private Type findType(Type type)
        {
            RegisterType(type);

            return this.types[type];
        }

        private Type findType(string fullname)
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

        private string generateId()
        {
            return Guid.NewGuid().ToString();
        }

        #region Class generation

        private Type generateType(Type type)
        {
            AssemblyName assemblyName = new AssemblyName("DynamicTypesAssembly");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, true);

			Type parent = null;

			if (typeof(IObject).IsAssignableFrom(type))
			{
				parent = typeof(ObjectBase);
			}
			else if (typeof(IStruct).IsAssignableFrom(type))
			{
				parent = typeof(StructBase);
			}
			else 
			{
				throw new NotSupportedException();
			}

            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                type.Name, // Name of class
                TypeAttributes.Public | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed, // Attributes
                parent, // Parent
                new Type[] { type } // Interfaces
                );

            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                generateProperty(property, typeBuilder);
            }

            return typeBuilder.CreateType();
        }

        private void generateProperty(PropertyInfo property, TypeBuilder typeBuilder)
        {
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
            if(typeof(INotifyPropertyChanged).IsAssignableFrom(property.PropertyType))
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

            ilSetGenerator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodSetBuilder, property.SetMethod);

            propertyBuilder.SetGetMethod(methodGetBuilder);
            propertyBuilder.SetSetMethod(methodSetBuilder);
        }

        #endregion

        #endregion
    }
}
