﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SisoDb.Annotations;
using SisoDb.Reflections;
using SisoDb.Resources;

namespace SisoDb.Structures.Schemas
{
    [Serializable]
    public class Property : IProperty
    {
        private Delegate _getter;
        private Delegate _setter;

        private static readonly Type UniqueAttributeType = typeof(UniqueAttribute);

        private List<PropertyInfo> Callstack { get; set; }

        public PropertyInfo Member { get; set; }

        public string Name
        {
            get { return Member.Name; }
        }

        public string Path { get; private set; }

        public Type PropertyType
        {
            get { return Member.PropertyType; }
        }

        public int Level { get; private set; }

        public IProperty Parent { get; private set; }

        public bool IsSimpleType { get; private set; }

        public bool IsUnique { get; private set; }

        public bool IsNullableValueType { get; private set; }

        public bool IsEnumerable { get; private set; }

        public bool IsElement { get; private set; }

        public Type ElementType { get; private set; }

        public Property(PropertyInfo member)
            : this(0, null, member)
        {
        }

        public Property(int level, IProperty parent, PropertyInfo member)
        {
            Member = member;
            Level = level;
            Parent = parent;

            IsSimpleType = Member.PropertyType.IsSimpleType();
            IsNullableValueType = Member.PropertyType.IsNullableValueType();
            IsEnumerable = Member.PropertyType.IsEnumerableType();
            ElementType = IsEnumerable ? Member.PropertyType.GetEnumerableElementType() : null;
            IsElement = Parent != null && (Parent.IsElement || Parent.IsEnumerable);
            
            var uniqueAttribute = (UniqueAttribute)Member.GetCustomAttributes(UniqueAttributeType, true).FirstOrDefault();
            if (uniqueAttribute != null && !IsSimpleType)
                throw new SisoDbException(ExceptionMessages.Property_Ctor_UniqueOnNonSimpleType);

            IsUnique = uniqueAttribute == null ? false : true;

            Path = PropertyPathBuilder.BuildPath(this);

            Callstack = GetCallstack(this);
            Callstack.Reverse();



            InitializeGetters();
            InitializeSetters();
        }

        private void InitializeGetters()
        {
            var type = Member.DeclaringType;
            var factoryClassType = typeof (X);

            if(!IsNullableValueType)
            {
                var getterFactory = factoryClassType.GetMethod("GetterFor").MakeGenericMethod(type, Member.PropertyType);
                _getter = (Delegate)getterFactory.Invoke(null, new object[] { Member });    
            }
            else
            {
                var getterFactory = factoryClassType.GetMethod("GetterForNullable").MakeGenericMethod(type, Member.PropertyType.GetGenericArguments()[0]);
                _getter = (Delegate)getterFactory.Invoke(null, new object[] { Member });   
            }
        }

        private void InitializeSetters()
        {
            if (Member.Name !=  StructureSchema.IdMemberName)
                return;

            var type = Member.DeclaringType;
            var factoryClassType = typeof(X);

            if (!IsNullableValueType)
            {
                var setterFactory = factoryClassType.GetMethod("SetterFor").MakeGenericMethod(type, Member.PropertyType);
                _setter = (Delegate)setterFactory.Invoke(null, new object[] { Member });
            }
            else
            {
                var setterFactory = factoryClassType.GetMethod("SetterForNullable").MakeGenericMethod(type, Member.PropertyType.GetGenericArguments()[0]);
                _setter = (Delegate)setterFactory.Invoke(null, new object[] { Member });
            }
        }

        private static List<PropertyInfo> GetCallstack(IProperty property)
        {
            if (property.Level == 0)
                return new List<PropertyInfo> { property.Member };

            var props = new List<PropertyInfo> { property.Member };
            var tmp = GetCallstack(property.Parent);
            props.AddRange(tmp);

            return props;
        }

        public TOut? GetIdValue<T, TOut>(T item)
            where T : class
            where TOut : struct
        {
            if (Level != 0)
                throw new SisoDbException(ExceptionMessages.Property_GetIdValue_InvalidLevel);

            return !Member.PropertyType.IsNullableValueType()
                       ? ((Func<T, TOut>) _getter).Invoke(item)
                       : ((Func<T, TOut?>)_getter).Invoke(item);
        }

        public void SetIdValue<T, TIn>(T item, TIn value)
            where T : class
            where TIn : struct
        {
            if (Level != 0)
                throw new SisoDbException(ExceptionMessages.Property_SetIdValue_InvalidLevel);

            if (!Member.PropertyType.IsNullableValueType())
                ((Action<T, TIn>)_setter).Invoke(item, value);
            else
                ((Action<T, TIn?>)_setter).Invoke(item, value);
        }

        public IList<object> GetValues(object item)
        {
            if (Level == 0)
            {
                var firstLevelPropValue = Member.GetValue(item, null);
                if (firstLevelPropValue == null)
                    return null;

                if (!IsEnumerable)
                    return new List<object> { firstLevelPropValue };

                var values = new List<object>();
                foreach (var value in (ICollection)firstLevelPropValue)
                    values.Add(value);

                return values;
            }

            return TraverseCallstack(item, 0);
        }

        private IList<object> TraverseCallstack<T>(T startNode, int startIndex)
        {
            object currentNode = startNode;
            for (var c = startIndex; c < Callstack.Count; c++)
            {
                if (currentNode == null)
                    return new object[] { null };

                var currentPropertyInfo = Callstack[c];
                var isLastPropertyInfo = c == (Callstack.Count - 1);
                if (isLastPropertyInfo)
                {
                    if (!(currentNode is ICollection))
                    {
                        var currentValue = currentPropertyInfo.GetValue(currentNode, null);
                        return new[] { currentValue };
                    }

                    var currentNodes = (ICollection)currentNode;
                    return ExtractValuesForEnumerableOfComplex(currentNodes, currentPropertyInfo);
                }

                if (!(currentNode is ICollection))
                    currentNode = currentPropertyInfo.GetValue(currentNode, null);
                else
                {
                    var currentNodes = (ICollection)currentNode;
                    var values = new List<object>();
                    foreach (var node in currentNodes)
                    {
                        var nodeValue = currentPropertyInfo.GetValue(node, null);
                        var tmp = TraverseCallstack(nodeValue, c + 1);
                        values.AddRange(tmp);
                    }
                    return values;
                }
            }

            return null;
        }

        private static IList<object> ExtractValuesForEnumerableOfComplex(ICollection nodes, PropertyInfo propertyAccessor)
        {
            var values = new List<object>();
            foreach (var node in nodes)
            {
                if (node == null)
                {
                    values.Add(null);
                    continue;
                }

                var nodeValue = propertyAccessor.GetValue(node, null);

                if (nodeValue == null || !(nodeValue is ICollection))
                    values.Add(nodeValue);
                else
                    foreach (var nodeValueElement in (ICollection)nodeValue)
                        values.Add(nodeValueElement);
            }

            return values;
        }
    }
}