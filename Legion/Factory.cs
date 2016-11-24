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
        public T create<T>() where T : class
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            //

            AssemblyName assemblyName = new AssemblyName("DynamicTypesAssembly");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, true);

            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                type.Name, // Name of class
                TypeAttributes.Public | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed, // Attributes
                typeof(object), // Parent
                new Type[] { type } // Interfaces
                );

            MethodAttributes getsetAttributes = 
                MethodAttributes.Public | 
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig | 
                MethodAttributes.Virtual | 
                MethodAttributes.NewSlot | 
                MethodAttributes.Final;

            foreach (PropertyInfo property in properties)
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
                ilSetGenerator.Emit(OpCodes.Ldarg_0);
                ilSetGenerator.Emit(OpCodes.Ldarg_1);
                ilSetGenerator.Emit(OpCodes.Stfld, fieldBuilder);
                ilSetGenerator.Emit(OpCodes.Ret);

                typeBuilder.DefineMethodOverride(methodSetBuilder, property.SetMethod);

                propertyBuilder.SetGetMethod(methodGetBuilder);
                propertyBuilder.SetSetMethod(methodSetBuilder);
            }

            Type generated = typeBuilder.CreateType();

            //

            return (T)Activator.CreateInstance(generated);
        }
    }
}
