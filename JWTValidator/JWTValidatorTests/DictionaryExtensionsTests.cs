using JWTValidatorService.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTValidatorTests;

[TestClass]
public class DictionaryExtensionsTests
{
    [TestMethod]
    public void DictionaryContainsKeyAndValueEmptyDictionaryTest()
    {
        //Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();
        String key = "role";
        String value = "member";

        //Act
        Boolean result = dictionary.DictionaryContainsKeyAndValue(key, value);

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DictionaryContainsKeyAndValueDictionaryDoesNotContainKeyTest()
    {
        //Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();
        dictionary.Add("hello", new List<String>
        {
            "member"
        });

        String key = "role";
        String value = "member";

        //Act
        Boolean result = dictionary.DictionaryContainsKeyAndValue(key, value);

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DictionaryContainsKeyAndValueDictionaryDoesNotContainValueTest()
    {
        //Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();
        dictionary.Add("role", new List<String>
        {
            "world"
        });

        String key = "role";
        String value = "member";

        //Act
        Boolean result = dictionary.DictionaryContainsKeyAndValue(key, value);

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DictionaryContainsKeyAndValueDictionaryContainsBothKeyAndValueTest()
    {
        //Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();
        dictionary.Add("role", new List<String>
        {
            "member"
        });

        String key = "role";
        String value = "member";

        //Act
        Boolean result = dictionary.DictionaryContainsKeyAndValue(key, value);

        //Assert
        Assert.IsTrue(result);
    }
}