//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Configuration;
namespace ManagedFusion.Rewriter.Configuration
{
    
    
    /// <summary>
    /// The ManagedFusionRewriterSectionGroup Configuration Section.
    /// </summary>
    public partial class ManagedFusionRewriterSectionGroup : global::System.Configuration.ConfigurationSection
    {
        
        #region Singleton Instance
        /// <summary>
        /// The XML name of the ManagedFusionRewriterSectionGroup Configuration Section.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string ManagedFusionRewriterSectionGroupSectionName = "managedFusion.rewriter";
        
        /// <summary>
        /// Gets the ManagedFusionRewriterSectionGroup instance.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        public static global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup Instance
        {
            get
            {
                return ((global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup)(global::System.Configuration.ConfigurationManager.GetSection(global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup.ManagedFusionRewriterSectionGroupSectionName)));
            }
        }
        #endregion
        
        #region Xmlns Property
        /// <summary>
        /// The XML name of the <see cref="Xmlns"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string XmlnsPropertyName = "xmlns";
        
        /// <summary>
        /// Gets the XML namespace of this Configuration Section.
        /// </summary>
        /// <remarks>
        /// This property makes sure that if the configuration file contains the XML namespace,
        /// the parser doesn't throw an exception because it encounters the unknown "xmlns" attribute.
        /// </remarks>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup.XmlnsPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false)]
        public string Xmlns
        {
            get
            {
                return ((string)(base[global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup.XmlnsPropertyName]));
            }
        }
        #endregion
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
        
        #region Azure Property
        /// <summary>
        /// The XML name of the <see cref="Azure"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string AzurePropertyName = "azure";
        
        /// <summary>
        /// Gets or sets the Rules.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The Azure Settings.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup.AzurePropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false)]
        public global::ManagedFusion.Rewriter.Configuration.AzureSection Azure
        {
            get
            {
                return ((global::ManagedFusion.Rewriter.Configuration.AzureSection)(base[global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup.AzurePropertyName]));
            }
            set
            {
                base[global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup.AzurePropertyName] = value;
            }
        }
        #endregion
        
        #region Rewriter Property
        /// <summary>
        /// The XML name of the <see cref="Rewriter"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string RewriterPropertyName = "rewriter";
        
        /// <summary>
        /// Gets or sets the Rewriter.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The Rewriter.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup.RewriterPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false)]
        public global::ManagedFusion.Rewriter.Configuration.RewriterSection Rewriter
        {
            get
            {
                return ((global::ManagedFusion.Rewriter.Configuration.RewriterSection)(base[global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup.RewriterPropertyName]));
            }
            set
            {
                base[global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup.RewriterPropertyName] = value;
            }
        }
        #endregion

		#region Proxy Property
		/// <summary>
		/// The XML name of the <see cref="Proxy"/> property.
		/// </summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
		internal const string ProxyPropertyName = "proxy";

		/// <summary>
		/// Gets or sets the Proxy.
		/// </summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
		[global::System.ComponentModel.DescriptionAttribute("The Proxy.")]
		[global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup.ProxyPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false)]
		public global::ManagedFusion.Rewriter.Configuration.ProxySection Proxy
		{
			get
			{
				return ((global::ManagedFusion.Rewriter.Configuration.ProxySection)(base[global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup.ProxyPropertyName]));
			}
			set
			{
				base[global::ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup.ProxyPropertyName] = value;
			}
		}
		#endregion
    }
}

namespace ManagedFusion.Rewriter.Configuration
{
    
    
    /// <summary>
    /// A collection of ApacheRuleSetItem instances.
    /// </summary>
    public partial class AzureSection : global::System.Configuration.ConfigurationElement
    {
        
        #region Constants
        /// <summary>
        /// The XML name of the individual <see cref="global::ManagedFusion.Rewriter.Configuration.ApacheRuleSetItem"/> instances in this collection.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string ApacheRuleSetItemPropertyName = "ruleSet";
        #endregion
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
        
        #region DefaultFileName Property
        /// <summary>
        /// The XML name of the <see cref="DefaultFileName"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string DefaultFileNamePropertyName = "defaultFileName";
        
        /// <summary>
        /// Gets or sets the DefaultFileName.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The DefaultFileName.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.AzureSection.DefaultFileNamePropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false, DefaultValue="ManagedFusion.Rewriter.txt")]
        public string DefaultFileName
        {
            get
            {
				return ((string)(base[global::ManagedFusion.Rewriter.Configuration.AzureSection.DefaultFileNamePropertyName]));
            }
            set
            {
				base[global::ManagedFusion.Rewriter.Configuration.AzureSection.DefaultFileNamePropertyName] = value;
            }
        }
        #endregion
        
        #region DefaultPhysicalApplicationPath Property
        /// <summary>
        /// The XML name of the <see cref="DefaultPhysicalApplicationPath"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string DefaultPhysicalApplicationPathPropertyName = "defaultPhysicalApplicationPath";
        
        /// <summary>
        /// Gets or sets the DefaultPhysicalApplicationPath.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The DefaultPhysicalApplicationPath.")]
		[global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.AzureSection.DefaultPhysicalApplicationPathPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false)]
        public string DefaultPhysicalApplicationPath
        {
            get
            {
				return ((string)(base[global::ManagedFusion.Rewriter.Configuration.AzureSection.DefaultPhysicalApplicationPathPropertyName]));
            }
            set
            {
				base[global::ManagedFusion.Rewriter.Configuration.AzureSection.DefaultPhysicalApplicationPathPropertyName] = value;
            }
        }
        #endregion

		[ConfigurationProperty("serviceNamespace", DefaultValue = null, IsRequired = true)]
		public string ServiceNamespace
		{
			get
			{
				return (string)this["serviceNamespace"];
			}
			set
			{
				this["serviceNamespace"] = value;
			}
		}

		[ConfigurationProperty("issuerName", DefaultValue = null, IsRequired = true)]
		public string IssuerName
		{
			get
			{
				return (string)this["issuerName"];
			}
			set
			{
				this["issuerName"] = value;
			}
		}

		[ConfigurationProperty("issuerSecret", DefaultValue = null, IsRequired = true)]
		public string IssuerSecret
		{
			get
			{
				return (string)this["issuerSecret"];
			}
			set
			{
				this["issuerSecret"] = value;
			}
		}
    }
}

namespace ManagedFusion.Rewriter.Configuration
{
    
    
    /// <summary>
    /// The ProxySection Configuration Element.
    /// </summary>
    public partial class ProxySection : global::System.Configuration.ConfigurationElement
    {
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
        
        #region UseAsyncProxy Property
        /// <summary>
        /// The XML name of the <see cref="UseAsyncProxy"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string UseAsyncProxyPropertyName = "useAsyncProxy";
        
        /// <summary>
        /// Gets or sets the UseAsyncProxy.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The UseAsyncProxy.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.ProxySection.UseAsyncProxyPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false, DefaultValue=true)]
        public bool UseAsyncProxy
        {
            get
            {
                return ((bool)(base[global::ManagedFusion.Rewriter.Configuration.ProxySection.UseAsyncProxyPropertyName]));
            }
            set
            {
                base[global::ManagedFusion.Rewriter.Configuration.ProxySection.UseAsyncProxyPropertyName] = value;
            }
        }
        #endregion
        
        #region BufferSize Property
        /// <summary>
        /// The XML name of the <see cref="BufferSize"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string BufferSizePropertyName = "bufferSize";
        
        /// <summary>
        /// Gets or sets the BufferSize.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The BufferSize.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.ProxySection.BufferSizePropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false, DefaultValue=4096)]
        public int BufferSize
        {
            get
            {
                return ((int)(base[global::ManagedFusion.Rewriter.Configuration.ProxySection.BufferSizePropertyName]));
            }
            set
            {
                base[global::ManagedFusion.Rewriter.Configuration.ProxySection.BufferSizePropertyName] = value;
            }
        }
        #endregion
        
        #region ResponseSize Property
        /// <summary>
        /// The XML name of the <see cref="ResponseSize"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string ResponseSizePropertyName = "responseSize";
        
        /// <summary>
        /// Gets or sets the ResponseSize.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The ResponseSize.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.ProxySection.ResponseSizePropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false, DefaultValue=2048)]
        public int ResponseSize
        {
            get
            {
                return ((int)(base[global::ManagedFusion.Rewriter.Configuration.ProxySection.ResponseSizePropertyName]));
            }
            set
            {
                base[global::ManagedFusion.Rewriter.Configuration.ProxySection.ResponseSizePropertyName] = value;
            }
        }
        #endregion
        
        #region RequestSize Property
        /// <summary>
        /// The XML name of the <see cref="RequestSize"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string RequestSizePropertyName = "requestSize";
        
        /// <summary>
        /// Gets or sets the RequestSize.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The RequestSize.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.ProxySection.RequestSizePropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false, DefaultValue=2048)]
        public int RequestSize
        {
            get
            {
                return ((int)(base[global::ManagedFusion.Rewriter.Configuration.ProxySection.RequestSizePropertyName]));
            }
            set
            {
                base[global::ManagedFusion.Rewriter.Configuration.ProxySection.RequestSizePropertyName] = value;
            }
        }
        #endregion
        
        #region ProxyType Property
        /// <summary>
        /// The XML name of the <see cref="ProxyType"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string ProxyTypePropertyName = "proxyType";
        
        /// <summary>
        /// Gets or sets the ProxyType.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The ProxyType.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.ProxySection.ProxyTypePropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false, DefaultValue="ManagedFusion.Rewriter.ProxyHandler, ManagedFusion.Rewriter")]
        public string ProxyType
        {
            get
            {
                return ((string)(base[global::ManagedFusion.Rewriter.Configuration.ProxySection.ProxyTypePropertyName]));
            }
            set
            {
                base[global::ManagedFusion.Rewriter.Configuration.ProxySection.ProxyTypePropertyName] = value;
            }
        }
        #endregion
        
        #region ProxyAsyncType Property
        /// <summary>
        /// The XML name of the <see cref="ProxyAsyncType"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string ProxyAsyncTypePropertyName = "proxyAsyncType";
        
        /// <summary>
        /// Gets or sets the ProxyAsyncType.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The ProxyAsyncType.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.ProxySection.ProxyAsyncTypePropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false, DefaultValue="ManagedFusion.Rewriter.ProxyAsyncHandler, ManagedFusion.Rewriter")]
        public string ProxyAsyncType
        {
            get
            {
                return ((string)(base[global::ManagedFusion.Rewriter.Configuration.ProxySection.ProxyAsyncTypePropertyName]));
            }
            set
            {
                base[global::ManagedFusion.Rewriter.Configuration.ProxySection.ProxyAsyncTypePropertyName] = value;
            }
        }
        #endregion
    }
}
namespace ManagedFusion.Rewriter.Configuration
{
    
    
    /// <summary>
    /// The RewriterSection Configuration Element.
    /// </summary>
    public partial class RewriterSection : global::System.Configuration.ConfigurationElement
    {
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
        
        #region TraceLog Property
        /// <summary>
        /// The XML name of the <see cref="TraceLog"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string TraceLogPropertyName = "traceLog";
        
        /// <summary>
        /// Gets or sets the TraceLog.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The TraceLog.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.RewriterSection.TraceLogPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false, DefaultValue=false)]
        public bool TraceLog
        {
            get
            {
                return ((bool)(base[global::ManagedFusion.Rewriter.Configuration.RewriterSection.TraceLogPropertyName]));
            }
            set
            {
                base[global::ManagedFusion.Rewriter.Configuration.RewriterSection.TraceLogPropertyName] = value;
            }
        }
        #endregion
        
        #region AllowIis7TransferRequest Property
        /// <summary>
        /// The XML name of the <see cref="AllowIis7TransferRequest"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string AllowIis7TransferRequestPropertyName = "allowIis7TransferRequest";
        
        /// <summary>
        /// Gets or sets the AllowIis7TransferRequest.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The AllowIis7TransferRequest.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.RewriterSection.AllowIis7TransferRequestPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false, DefaultValue=true)]
        public bool AllowIis7TransferRequest
        {
            get
            {
                return ((bool)(base[global::ManagedFusion.Rewriter.Configuration.RewriterSection.AllowIis7TransferRequestPropertyName]));
            }
            set
            {
                base[global::ManagedFusion.Rewriter.Configuration.RewriterSection.AllowIis7TransferRequestPropertyName] = value;
            }
        }
        #endregion
        
        #region AllowVanityHeader Property
        /// <summary>
        /// The XML name of the <see cref="AllowVanityHeader"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string AllowVanityHeaderPropertyName = "allowVanityHeader";
        
        /// <summary>
        /// Gets or sets the AllowVanityHeader.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The AllowVanityHeader.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.RewriterSection.AllowVanityHeaderPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false, DefaultValue=true)]
        public bool AllowVanityHeader
        {
            get
            {
                return ((bool)(base[global::ManagedFusion.Rewriter.Configuration.RewriterSection.AllowVanityHeaderPropertyName]));
            }
            set
            {
                base[global::ManagedFusion.Rewriter.Configuration.RewriterSection.AllowVanityHeaderPropertyName] = value;
            }
        }
        #endregion
        
        #region AllowXRewriteUrlHeader Property
        /// <summary>
        /// The XML name of the <see cref="AllowXRewriteUrlHeader"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string AllowXRewriteUrlHeaderPropertyName = "allowXRewriteUrlHeader";
        
        /// <summary>
        /// Gets or sets the AllowXRewriteUrlHeader.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The AllowXRewriteUrlHeader.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.RewriterSection.AllowXRewriteUrlHeaderPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false, DefaultValue=true)]
        public bool AllowXRewriteUrlHeader
        {
            get
            {
                return ((bool)(base[global::ManagedFusion.Rewriter.Configuration.RewriterSection.AllowXRewriteUrlHeaderPropertyName]));
            }
            set
            {
                base[global::ManagedFusion.Rewriter.Configuration.RewriterSection.AllowXRewriteUrlHeaderPropertyName] = value;
            }
        }
        #endregion
        
        #region AllowRequestHeaderAdding Property
        /// <summary>
        /// The XML name of the <see cref="AllowRequestHeaderAdding"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        internal const string AllowRequestHeaderAddingPropertyName = "allowRequestHeaderAdding";
        
        /// <summary>
        /// Gets or sets the AllowRequestHeaderAdding.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "1.6.1.0")]
        [global::System.ComponentModel.DescriptionAttribute("The AllowRequestHeaderAdding.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::ManagedFusion.Rewriter.Configuration.RewriterSection.AllowRequestHeaderAddingPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false, DefaultValue=true)]
        public bool AllowRequestHeaderAdding
        {
            get
            {
                return ((bool)(base[global::ManagedFusion.Rewriter.Configuration.RewriterSection.AllowRequestHeaderAddingPropertyName]));
            }
            set
            {
                base[global::ManagedFusion.Rewriter.Configuration.RewriterSection.AllowRequestHeaderAddingPropertyName] = value;
            }
        }
        #endregion
    }
}
