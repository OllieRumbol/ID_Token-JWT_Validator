using JWTValidatorService.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
        Boolean result = dictionary.ContainsKeyAndValue(key, value);

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
        Boolean result = dictionary.ContainsKeyAndValue(key, value);

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
        Boolean result = dictionary.ContainsKeyAndValue(key, value);

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
        Boolean result = dictionary.ContainsKeyAndValue(key, value);

        //Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void DictionaryContainsValueInList_EmptyList_ReturnsFalse()
    {
        //Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();

        //Act
        Boolean result = dictionary.ContainsValueInList("test123");

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DictionaryContainsValueInList_NotInList_ReturnsFalse()
    {
        //Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();
        dictionary.Add("role", new List<String>
        {
            "member"
        });
        dictionary.Add("hello", new List<String>
        {
            "world",
            "12345678"
        });
        dictionary.Add("example", new List<String>
        {
            "test1",
            "test2",
            "test3"
        });

        //Act
        Boolean result = dictionary.ContainsValueInList("test123");

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DictionaryIsEmpty_HasValues_ShouldBeFalse()
    {
        //Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();
        dictionary.Add("role", new List<String>
        {
            "member"
        });
        dictionary.Add("hello", new List<String>
        {
            "world",
            "12345678"
        });
        dictionary.Add("example", new List<String>
        {
            "test1",
            "test2",
            "test3"
        });

        //Act
        Boolean result = dictionary.IsEmpty();

        //Assert
        Assert.IsFalse(result);
    }
}