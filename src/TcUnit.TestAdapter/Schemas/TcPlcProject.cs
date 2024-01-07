namespace TcUnit.TestAdapter.Schemas
{
	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", IsNullable = false)]
	public partial class Project
	{

		private ProjectPropertyGroup propertyGroupField;

		private ProjectItemGroup[] itemGroupField;

		private ProjectProjectExtensions projectExtensionsField;

		private string defaultTargetsField;

		/// <remarks/>
		public ProjectPropertyGroup PropertyGroup {
			get {
				return this.propertyGroupField;
			}
			set {
				this.propertyGroupField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("ItemGroup")]
		public ProjectItemGroup[] ItemGroup {
			get {
				return this.itemGroupField;
			}
			set {
				this.itemGroupField = value;
			}
		}

		/// <remarks/>
		public ProjectProjectExtensions ProjectExtensions {
			get {
				return this.projectExtensionsField;
			}
			set {
				this.projectExtensionsField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string DefaultTargets {
			get {
				return this.defaultTargetsField;
			}
			set {
				this.defaultTargetsField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectPropertyGroup
	{

		private string fileVersionField;

		private decimal schemaVersionField;

		private string projectGuidField;

		private string subObjectsSortedByNameField;

		private bool downloadApplicationInfoField;

		private bool writeProductVersionField;

		private bool generateTpyField;

		private string nameField;

		private string programVersionField;

		private string applicationField;

		private string typeSystemField;

		private string implicit_Task_InfoField;

		private string implicit_KindOfTaskField;

		private string implicit_Jitter_DistributionField;

		private string libraryReferencesField;

		/// <remarks/>
		public string FileVersion {
			get {
				return this.fileVersionField;
			}
			set {
				this.fileVersionField = value;
			}
		}

		/// <remarks/>
		public decimal SchemaVersion {
			get {
				return this.schemaVersionField;
			}
			set {
				this.schemaVersionField = value;
			}
		}

		/// <remarks/>
		public string ProjectGuid {
			get {
				return this.projectGuidField;
			}
			set {
				this.projectGuidField = value;
			}
		}

		/// <remarks/>
		public string SubObjectsSortedByName {
			get {
				return this.subObjectsSortedByNameField;
			}
			set {
				this.subObjectsSortedByNameField = value;
			}
		}

		/// <remarks/>
		public bool DownloadApplicationInfo {
			get {
				return this.downloadApplicationInfoField;
			}
			set {
				this.downloadApplicationInfoField = value;
			}
		}

		/// <remarks/>
		public bool WriteProductVersion {
			get {
				return this.writeProductVersionField;
			}
			set {
				this.writeProductVersionField = value;
			}
		}

		/// <remarks/>
		public bool GenerateTpy {
			get {
				return this.generateTpyField;
			}
			set {
				this.generateTpyField = value;
			}
		}

		/// <remarks/>
		public string Name {
			get {
				return this.nameField;
			}
			set {
				this.nameField = value;
			}
		}

		/// <remarks/>
		public string ProgramVersion {
			get {
				return this.programVersionField;
			}
			set {
				this.programVersionField = value;
			}
		}

		/// <remarks/>
		public string Application {
			get {
				return this.applicationField;
			}
			set {
				this.applicationField = value;
			}
		}

		/// <remarks/>
		public string TypeSystem {
			get {
				return this.typeSystemField;
			}
			set {
				this.typeSystemField = value;
			}
		}

		/// <remarks/>
		public string Implicit_Task_Info {
			get {
				return this.implicit_Task_InfoField;
			}
			set {
				this.implicit_Task_InfoField = value;
			}
		}

		/// <remarks/>
		public string Implicit_KindOfTask {
			get {
				return this.implicit_KindOfTaskField;
			}
			set {
				this.implicit_KindOfTaskField = value;
			}
		}

		/// <remarks/>
		public string Implicit_Jitter_Distribution {
			get {
				return this.implicit_Jitter_DistributionField;
			}
			set {
				this.implicit_Jitter_DistributionField = value;
			}
		}

		/// <remarks/>
		public string LibraryReferences {
			get {
				return this.libraryReferencesField;
			}
			set {
				this.libraryReferencesField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectItemGroup
	{

		private ProjectItemGroupContent contentField;

		private ProjectItemGroupNone noneField;

		private ProjectItemGroupLibraryReference[] libraryReferenceField;

		private ProjectItemGroupPlaceholderResolution[] placeholderResolutionField;

		private ProjectItemGroupPlaceholderReference[] placeholderReferenceField;

		private ProjectItemGroupFolder[] folderField;

		private ProjectItemGroupCompile[] compileField;

		/// <remarks/>
		public ProjectItemGroupContent Content {
			get {
				return this.contentField;
			}
			set {
				this.contentField = value;
			}
		}

		/// <remarks/>
		public ProjectItemGroupNone None {
			get {
				return this.noneField;
			}
			set {
				this.noneField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("LibraryReference")]
		public ProjectItemGroupLibraryReference[] LibraryReference {
			get {
				return this.libraryReferenceField;
			}
			set {
				this.libraryReferenceField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("PlaceholderResolution")]
		public ProjectItemGroupPlaceholderResolution[] PlaceholderResolution {
			get {
				return this.placeholderResolutionField;
			}
			set {
				this.placeholderResolutionField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("PlaceholderReference")]
		public ProjectItemGroupPlaceholderReference[] PlaceholderReference {
			get {
				return this.placeholderReferenceField;
			}
			set {
				this.placeholderReferenceField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("Folder")]
		public ProjectItemGroupFolder[] Folder {
			get {
				return this.folderField;
			}
			set {
				this.folderField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("Compile")]
		public ProjectItemGroupCompile[] Compile {
			get {
				return this.compileField;
			}
			set {
				this.compileField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectItemGroupContent
	{

		private string subTypeField;

		private string includeField;

		/// <remarks/>
		public string SubType {
			get {
				return this.subTypeField;
			}
			set {
				this.subTypeField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Include {
			get {
				return this.includeField;
			}
			set {
				this.includeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectItemGroupNone
	{

		private string subTypeField;

		private string includeField;

		/// <remarks/>
		public string SubType {
			get {
				return this.subTypeField;
			}
			set {
				this.subTypeField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Include {
			get {
				return this.includeField;
			}
			set {
				this.includeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectItemGroupLibraryReference
	{

		private string namespaceField;

		private ProjectItemGroupLibraryReferenceParameter[] parametersField;

		private string includeField;

		/// <remarks/>
		public string Namespace {
			get {
				return this.namespaceField;
			}
			set {
				this.namespaceField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItemAttribute("Parameter", IsNullable = false)]
		public ProjectItemGroupLibraryReferenceParameter[] Parameters {
			get {
				return this.parametersField;
			}
			set {
				this.parametersField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Include {
			get {
				return this.includeField;
			}
			set {
				this.includeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectItemGroupLibraryReferenceParameter
	{

		private string keyField;

		private string valueField;

		private string listNameField;

		/// <remarks/>
		public string Key {
			get {
				return this.keyField;
			}
			set {
				this.keyField = value;
			}
		}

		/// <remarks/>
		public string Value {
			get {
				return this.valueField;
			}
			set {
				this.valueField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string ListName {
			get {
				return this.listNameField;
			}
			set {
				this.listNameField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectItemGroupPlaceholderResolution
	{

		private string resolutionField;

		private string includeField;

		/// <remarks/>
		public string Resolution {
			get {
				return this.resolutionField;
			}
			set {
				this.resolutionField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Include {
			get {
				return this.includeField;
			}
			set {
				this.includeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectItemGroupPlaceholderReference
	{

		private string defaultResolutionField;

		private string namespaceField;

		private ProjectItemGroupPlaceholderReferenceParameter[] parametersField;

		private bool systemLibraryField;

		private bool systemLibraryFieldSpecified;

		private string resolverGuidField;

		private string includeField;

		/// <remarks/>
		public string DefaultResolution {
			get {
				return this.defaultResolutionField;
			}
			set {
				this.defaultResolutionField = value;
			}
		}

		/// <remarks/>
		public string Namespace {
			get {
				return this.namespaceField;
			}
			set {
				this.namespaceField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItemAttribute("Parameter", IsNullable = false)]
		public ProjectItemGroupPlaceholderReferenceParameter[] Parameters {
			get {
				return this.parametersField;
			}
			set {
				this.parametersField = value;
			}
		}

		/// <remarks/>
		public bool SystemLibrary {
			get {
				return this.systemLibraryField;
			}
			set {
				this.systemLibraryField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlIgnoreAttribute()]
		public bool SystemLibrarySpecified {
			get {
				return this.systemLibraryFieldSpecified;
			}
			set {
				this.systemLibraryFieldSpecified = value;
			}
		}

		/// <remarks/>
		public string ResolverGuid {
			get {
				return this.resolverGuidField;
			}
			set {
				this.resolverGuidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Include {
			get {
				return this.includeField;
			}
			set {
				this.includeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectItemGroupPlaceholderReferenceParameter
	{

		private string keyField;

		private string valueField;

		private string listNameField;

		/// <remarks/>
		public string Key {
			get {
				return this.keyField;
			}
			set {
				this.keyField = value;
			}
		}

		/// <remarks/>
		public string Value {
			get {
				return this.valueField;
			}
			set {
				this.valueField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string ListName {
			get {
				return this.listNameField;
			}
			set {
				this.listNameField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectItemGroupFolder
	{

		private string includeField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Include {
			get {
				return this.includeField;
			}
			set {
				this.includeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectItemGroupCompile
	{

		private string subTypeField;

		private string dependentUponField;

		private string includeField;

		/// <remarks/>
		public string SubType {
			get {
				return this.subTypeField;
			}
			set {
				this.subTypeField = value;
			}
		}

		/// <remarks/>
		public string DependentUpon {
			get {
				return this.dependentUponField;
			}
			set {
				this.dependentUponField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Include {
			get {
				return this.includeField;
			}
			set {
				this.includeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectProjectExtensions
	{

		private ProjectProjectExtensionsPlcProjectOptions plcProjectOptionsField;

		/// <remarks/>
		public ProjectProjectExtensionsPlcProjectOptions PlcProjectOptions {
			get {
				return this.plcProjectOptionsField;
			}
			set {
				this.plcProjectOptionsField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectProjectExtensionsPlcProjectOptions
	{

		private ProjectProjectExtensionsPlcProjectOptionsXmlArchive xmlArchiveField;

		/// <remarks/>
		public ProjectProjectExtensionsPlcProjectOptionsXmlArchive XmlArchive {
			get {
				return this.xmlArchiveField;
			}
			set {
				this.xmlArchiveField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectProjectExtensionsPlcProjectOptionsXmlArchive
	{

		private ProjectProjectExtensionsPlcProjectOptionsXmlArchiveData dataField;

		private ProjectProjectExtensionsPlcProjectOptionsXmlArchiveType[] typeListField;

		/// <remarks/>
		public ProjectProjectExtensionsPlcProjectOptionsXmlArchiveData Data {
			get {
				return this.dataField;
			}
			set {
				this.dataField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItemAttribute("Type", IsNullable = false)]
		public ProjectProjectExtensionsPlcProjectOptionsXmlArchiveType[] TypeList {
			get {
				return this.typeListField;
			}
			set {
				this.typeListField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectProjectExtensionsPlcProjectOptionsXmlArchiveData
	{

		private ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataO oField;

		/// <remarks/>
		public ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataO o {
			get {
				return this.oField;
			}
			set {
				this.oField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataO
	{

		private ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataOV vField;

		private ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataOD[] dField;

		private string[] textField;

		private string spaceField;

		private string tField;

		/// <remarks/>
		public ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataOV v {
			get {
				return this.vField;
			}
			set {
				this.vField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("d")]
		public ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataOD[] d {
			get {
				return this.dField;
			}
			set {
				this.dField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlTextAttribute()]
		public string[] Text {
			get {
				return this.textField;
			}
			set {
				this.textField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
		public string space {
			get {
				return this.spaceField;
			}
			set {
				this.spaceField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string t {
			get {
				return this.tField;
			}
			set {
				this.tField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataOV
	{

		private string nField;

		private string valueField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string n {
			get {
				return this.nField;
			}
			set {
				this.nField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlTextAttribute()]
		public string Value {
			get {
				return this.valueField;
			}
			set {
				this.valueField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataOD
	{

		private object[] itemsField;

		private string[] textField;

		private string nField;

		private string tField;

		private string cktField;

		private string cvtField;

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("o", typeof(ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataODO))]
		[System.Xml.Serialization.XmlElementAttribute("v", typeof(string))]
		public object[] Items {
			get {
				return this.itemsField;
			}
			set {
				this.itemsField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlTextAttribute()]
		public string[] Text {
			get {
				return this.textField;
			}
			set {
				this.textField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string n {
			get {
				return this.nField;
			}
			set {
				this.nField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string t {
			get {
				return this.tField;
			}
			set {
				this.tField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string ckt {
			get {
				return this.cktField;
			}
			set {
				this.cktField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string cvt {
			get {
				return this.cvtField;
			}
			set {
				this.cvtField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataODO
	{

		private ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataODOV vField;

		private ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataODOD[] dField;

		private string[] textField;

		/// <remarks/>
		public ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataODOV v {
			get {
				return this.vField;
			}
			set {
				this.vField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("d")]
		public ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataODOD[] d {
			get {
				return this.dField;
			}
			set {
				this.dField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlTextAttribute()]
		public string[] Text {
			get {
				return this.textField;
			}
			set {
				this.textField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataODOV
	{

		private string nField;

		private string valueField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string n {
			get {
				return this.nField;
			}
			set {
				this.nField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlTextAttribute()]
		public string Value {
			get {
				return this.valueField;
			}
			set {
				this.valueField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectProjectExtensionsPlcProjectOptionsXmlArchiveDataODOD
	{

		private string[] vField;

		private string[] textField;

		private string nField;

		private string tField;

		private string cktField;

		private string cvtField;

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("v")]
		public string[] v {
			get {
				return this.vField;
			}
			set {
				this.vField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlTextAttribute()]
		public string[] Text {
			get {
				return this.textField;
			}
			set {
				this.textField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string n {
			get {
				return this.nField;
			}
			set {
				this.nField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string t {
			get {
				return this.tField;
			}
			set {
				this.tField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string ckt {
			get {
				return this.cktField;
			}
			set {
				this.cktField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string cvt {
			get {
				return this.cvtField;
			}
			set {
				this.cvtField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	public partial class ProjectProjectExtensionsPlcProjectOptionsXmlArchiveType
	{

		private string nField;

		private string valueField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string n {
			get {
				return this.nField;
			}
			set {
				this.nField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlTextAttribute()]
		public string Value {
			get {
				return this.valueField;
			}
			set {
				this.valueField = value;
			}
		}
	}


}

