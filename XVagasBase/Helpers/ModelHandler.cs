using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Base.Entities;
using Microsoft.EntityFrameworkCore;

namespace Base.Helpers
{
    public static class ModelHandler
    {
        /// <summary>
        /// This will return a model based on the typed that is being passed
        /// </summary>
        /// <example>UModel newModel = TModel.ConvertTo<UModel>()</example>
        public static TConvert ConvertTo<TConvert>(this object entity) where TConvert : new()
        {
            var convertProperties = TypeDescriptor.GetProperties(typeof(TConvert)).Cast<PropertyDescriptor>();
            var entityProperties = TypeDescriptor.GetProperties(entity).Cast<PropertyDescriptor>();

            var convert = new TConvert();

            foreach (var entityProperty in entityProperties)
            {
                var property = entityProperty;
                var convertProperty = convertProperties.FirstOrDefault(prop => prop.Name == property.Name);
                if (convertProperty != null)
                {
                    if (Nullable.GetUnderlyingType(convertProperty.PropertyType) != null)
                    {
                        convertProperty.SetValue(convert, Convert.ChangeType(entityProperty.GetValue(entity), Nullable.GetUnderlyingType(convertProperty.PropertyType)));
                    }
                    else
                    {
                        convertProperty.SetValue(convert, Convert.ChangeType(entityProperty.GetValue(entity), convertProperty.PropertyType));
                    }
                }
            }

            return convert;
        }

        /// <summary>
        /// Copies the properties that are not null to the new object
        /// </summary>
        /// <example>TModel.copyPropertiesTo(UDestModel)</example>
        public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
        {
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            var destProps = typeof(TU).GetProperties()
                    .Where(x => x.CanWrite)
                    .ToList();

            foreach (var sourceProp in sourceProps)
            {
                if (destProps.Any(x => x.Name == sourceProp.Name)
                    && sourceProp.GetValue(source, null) != null
                    && sourceProp.GetValue(source).ToString() != "0"
                    && sourceProp.GetValue(source).ToString().Trim() != String.Empty)
                {
                    var p = destProps.First(x => x.Name == sourceProp.Name);
                    p.SetValue(dest, sourceProp.GetValue(source, null), null);
                }

            }

        }


        public static IQueryable<T> LoadRelated<T>(this IQueryable<T> originalQuery) where T : BaseEntity, new()
        {
            Func<IQueryable<T>, IQueryable<T>> includeFunc = f => f;
            foreach (var prop in typeof(T).GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(IncludeAttribute))))
            {
                //System.Diagnostics.Debug.WriteLine("==>" + prop.Name + "\n");
                Func<IQueryable<T>, IQueryable<T>> chainedIncludeFunc = f => f.Include(prop.Name);
                includeFunc = Compose(includeFunc, chainedIncludeFunc);
            }
            
            return includeFunc(originalQuery);
        }

        public static IQueryable<T> DefaultWhere<T, VO>(this IQueryable<T> originalQuery, VO filter) where T : BaseEntity, new()
        {            
            Expression queryExpr = originalQuery.Expression;
            var parameter = Expression.Parameter(typeof(T),"o"); 
            foreach (var prop in typeof(VO).GetProperties())      
            {
                Type typeNullable = null;
                Expression val;
                var valueFilter = GetAttributeValue<VO>(filter, prop.Name);                
                
                if(valueFilter!=null && prop.Name!="id" && prop.Name!="orderBy" && prop.Name!="way")
                {             
                    System.Diagnostics.Debug.WriteLine("==>"+prop.Name);                           
                    if(typeof(VO).GetProperty(prop.Name).PropertyType.ToString().Contains("Nullable"))
                    {                        
                        typeNullable = (GetAttributeValue<VO>(filter, prop.Name).GetType().GetNullableType());                                        
                        val = Expression.Constant(valueFilter, typeNullable);
                    }
                    else
                    {
                        val = Expression.Constant(valueFilter);
                    }                      
                    
                    var propr = Expression.Property(parameter, prop.Name);   
                    var body = Expression.Equal(propr, val);                    
                    var selectorExp =  Expression.Lambda<Func<T, bool>>(body, parameter);                    
                    originalQuery = System.Linq.Queryable.Where(originalQuery, selectorExp);
                }
            }
            return originalQuery;          
            
        }
        
        public static IQueryable<T> DefaulSort<T, VO>(this IQueryable<T> originalQuery, VO filter) where T : BaseEntity, new()
        { 
            Expression queryExpr = originalQuery.Expression;            
            string valueFilter = (string) GetAttributeValue<VO>(filter, "orderBy");  
            if(valueFilter!=null)
            {
                var parameter = Expression.Parameter(typeof(T),"o"); 
                var propr = Expression.Lambda<Func<T, object>>
                            (Expression.Convert(Expression.Property(parameter,  GetPropertyInfoSort<T>(valueFilter)), typeof(object)), parameter);
                //var propr = Expression.Property(parameter, GetPropertyInfoSort (filter, (string) valueFilter)); 
                if((string) GetAttributeValue<VO>(filter, "way")=="ASC")
                {
                    originalQuery = System.Linq.Queryable.OrderBy(originalQuery, propr);
                    //originalQuery.AsQueryable<T>().OrderBy<T, object>(propr);
                }
                else
                {
                    originalQuery = System.Linq.Queryable.OrderByDescending(originalQuery, propr);
                    //originalQuery.AsQueryable<T>().OrderByDescending<T, object>(propr);
                }
            }           
            else
            {
                originalQuery = System.Linq.Queryable.OrderByDescending(originalQuery, o=>o.Id);
                //originalQuery.AsQueryable<T>().OrderBy(m=> m.Id);              
            }
            
            
            return originalQuery;  
        }

        public static PropertyInfo GetPropertyInfoSort<V> (string target)
        {
            foreach (var prop in typeof(V).GetProperties())      
            {
                Debug.WriteLine(prop.Name);
                if(prop.Name.ToUpper() == target.ToUpper())
                    return prop;
            }
            return null;
        }

        public static Type GetNullableType(this Type TypeToConvert)
        {
            
            // Abort if no type supplied
            if (TypeToConvert == null)
                return null;

            // If the given type is already nullable, just return it
            if (IsTypeNullable(TypeToConvert))
                return TypeToConvert;

            // If the type is a ValueType and is not System.Void, convert it to a Nullable<Type>
            if (TypeToConvert.IsValueType && TypeToConvert != typeof(void))
                return typeof(Nullable<>).MakeGenericType(TypeToConvert);

            // Done - no conversion
            return null;
        }        

        public static bool IsTypeNullable(Type TypeToTest)
        {
            // Abort if no type supplied
            if (TypeToTest == null)
                return false;

            // If this is not a value type, it is a reference type, so it is automatically nullable
            //  (NOTE: All forms of Nullable<T> are value types)
            if (!TypeToTest.IsValueType)
                return true;

            // Report whether TypeToTest is a form of the Nullable<> type
            return Nullable.GetUnderlyingType(TypeToTest) != null;
        }
        public static object GetPropValue(object source, string propertyName)
        {
            var property = source.GetType().GetRuntimeProperties().FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));
            return property?.GetValue(source);
        }

        public static object GetAttributeValue<T>(T element, string attribute)
        {
            try
            {
                return element.GetType().GetProperty(attribute).GetValue(element, null);
            }
            catch (Exception e)
            { //Attribute not found
                Console.WriteLine(e.Message);
                Console.WriteLine("Attribute '" + attribute + "' not found in '" + element.GetType() + "'");
                return null;
            }
        }

        private static Func<T, T> Compose<T>(Func<T, T> innerFunc, Func<T, T> outerFunc)
        {
            return arg => outerFunc(innerFunc(arg));
        }

    }
}
