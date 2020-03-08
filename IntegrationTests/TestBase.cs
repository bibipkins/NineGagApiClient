using IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.IO;

namespace IntegrationTest
{
    public abstract class TestBase
    {
        public Settings Settings { get; private set; }

        [TestInitialize]
        public void TestInit()
        {
            try
            {
                var settingsJson = JObject.Parse(File.ReadAllText(@"settings.json"));
                var generator = new JSchemaGenerator();
                var schema = generator.Generate(typeof(Settings));

                bool areSettingsValid = settingsJson.IsValid(schema, out IList<string> errors);
                Assert.IsTrue(areSettingsValid, "\n - " + string.Join("\n - ", errors));

                Settings = settingsJson.ToObject<Settings>();
            }
            catch (Exception e)
            {
                throw new AssertFailedException(
                    "Check if the settings.json is correct and compliant with Settings.cs class!" +
                    "\nIf you do not have credentials register at https://9gag.com/" +
                    $"\n{e.Message}"
                );
            }
        }
    }
}
