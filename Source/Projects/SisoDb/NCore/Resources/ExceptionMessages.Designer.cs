﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17626
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SisoDb.NCore.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SisoDb.NCore.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Don&apos;t know how to evaluate the expression type: &apos;{0}&apos;..
        /// </summary>
        public static string ExpressionEvaluation_DontKnowHowToEvalExpression {
            get {
                return ResourceManager.GetString("ExpressionEvaluation_DontKnowHowToEvalExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Don&apos;t know how to evaluate the unary expression of node type: &apos;{0}&apos;..
        /// </summary>
        public static string ExpressionEvaluation_DontKnowHowToEvalUnaryExpression {
            get {
                return ResourceManager.GetString("ExpressionEvaluation_DontKnowHowToEvalUnaryExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value being retrieved is not of supported type. It must either be a string or the type needs to implement IConvertible or if Nullable&lt;T&gt;, then T needs to implement it..
        /// </summary>
        public static string StringConverter_AsString_TypeOfValueIsNotSupported {
            get {
                return ResourceManager.GetString("StringConverter_AsString_TypeOfValueIsNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to When extracting generic element type from enumerables, a maximum of two generic arguments are supported, which then are supposed to belong to KeyValuePair&lt;TKey, TValue&gt;..
        /// </summary>
        public static string TypeExtensions_ExtractEnumerableGenericType {
            get {
                return ResourceManager.GetString("TypeExtensions_ExtractEnumerableGenericType", resourceCulture);
            }
        }
    }
}