using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The unit test suite for JSON schema description.
/// </summary>
[TestClass]
public class SchemaUnitTest
{
    /// <summary>
    /// Tests JSON schema.
    /// </summary>
    [TestMethod]
    public void TestJsonSchema()
    {
        var schema = JsonValues.CreateSchema<JsonModel>("Unit test.") as JsonObjectSchemaDescription;
        Assert.IsNotNull(schema);
        Assert.AreEqual("Unit test.", schema.Description);
        Assert.AreEqual(5, schema.Properties.Count);
        Assert.AreEqual("Property A.", schema.Properties["str-a"].Description);
        Assert.AreEqual(typeof(JsonStringSchemaDescription), schema.Properties["str-a"].GetType());
        Assert.IsNull(schema.Properties["str-c"].Description);
        var arrSchema = schema.Properties["arr"] as JsonArraySchemaDescription;
        Assert.IsNotNull(arrSchema?.DefaultItems);
        Assert.AreEqual(typeof(JsonIntegerSchemaDescription), arrSchema.DefaultItems.GetType());
    }
}
