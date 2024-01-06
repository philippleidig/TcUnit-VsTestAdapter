using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Resources;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TcUnit.TestAdapter.RunSettings
{
    [SettingsName(TestAdapter.RunSettingsName)]
    [Export(typeof(ISettingsProvider))]
    public class RunSettingsProvider : ISettingsProvider
    {
        protected readonly XmlSerializer serializer;

        public TestSettings Settings { get; private set; }
        public string Name { get; private set; }

        public RunSettingsProvider()
        {
            Name = TestAdapter.RunSettingsName;
            Settings = new TestSettings();
            serializer = new XmlSerializer(typeof(TestSettings));
        }

        public void Load(XmlReader reader)
        {
            ValidateArg.NotNull(reader, "reader");

            var schemaSet = new XmlSchemaSet();
            var schemaStream = Assembly.GetExecutingAssembly()
                                        .GetManifestResourceStream("TcUnit.TestAdapter.Schemas.TestSettingsXmlSchema.xsd");

            schemaSet.Add(null, XmlReader.Create(schemaStream));

            var settings = new XmlReaderSettings
            {
                Schemas = schemaSet,
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
            };

            settings.ValidationEventHandler += (object o, ValidationEventArgs e) => throw e.Exception;

            using (var newReader = XmlReader.Create(reader, settings))
            {
                try
                {
                    if (newReader.Read() && newReader.Name.Equals(Name))
                    {
                        Settings = serializer.Deserialize(newReader) as TestSettings;
                    }  
                }
                catch (XmlSchemaValidationException e) 
                {
                    throw new InvalidTestSettingsException(e.Message, e);
                }
                catch (InvalidOperationException e) when (e.InnerException is XmlSchemaValidationException)
                {
                    throw new InvalidTestSettingsException(e.InnerException.Message, e.InnerException);
                }
            }

        }
    }

    [Serializable]
    public class InvalidTestSettingsException : Exception
    {
        public InvalidTestSettingsException() { }
        public InvalidTestSettingsException(string message) : base(message) { }
        public InvalidTestSettingsException(string message, Exception inner) : base(message, inner) { }
        protected InvalidTestSettingsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
