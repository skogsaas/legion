using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Legion
{
    public class Factory
    {
        private Dictionary<Type /* Interface */, Type /* Generated */> types;

        public Factory()
        {
            this.types = new Dictionary<Type, Type>();
        }

        public T create<T>() where T : class
        {
            Type type = typeof(T);
            Type generated = null;

            if(this.types.ContainsKey(type))
            {
                generated = this.types[type];
            }
            else
            {
                generated = this.types[type] = generateType(type);
            }

            return (T)Activator.CreateInstance(generated);
        }

        private Type generateType(Type type)
        {
            AssemblyName assemblyName = new AssemblyName("DynamicTypesAssembly");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, true);

            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                type.Name, // Name of class
                TypeAttributes.Public | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed, // Attributes
                typeof(ObjectBase), // Parent
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
            ilSetGenerator.Emit(OpCodes.Nop);
            // Now call the NotifyPropertyChanged-function in the base class.
            ilSetGenerator.Emit(OpCodes.Ldarg_0); // "this"
            ilSetGenerator.Emit(OpCodes.Ldstr, property.Name);
            ilSetGenerator.Emit(OpCodes.Callvirt, typeof(ObjectBase).GetMethod("NotifyPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic));
            ilSetGenerator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodSetBuilder, property.SetMethod);

            propertyBuilder.SetGetMethod(methodGetBuilder);
            propertyBuilder.SetSetMethod(methodSetBuilder);
        }
    }
}
