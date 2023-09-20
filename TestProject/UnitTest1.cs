using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Converters;
using Host.Interfaces;
using Host.Services;

namespace TestProject;

public class Tests
{
    private LongTitleConverter _longTitleconverter;
    private ClassWithParameters _parametrizedClass;

    [SetUp]
    public void Setup()
    {
        _longTitleconverter = new LongTitleConverter();
        _parametrizedClass = new ClassWithParameters();
    }

    [Test]
    public void LongTitleConverter_Max8_CorrectOutput()
    {
        const string TEST = "Test string for converter";

        var output = _longTitleconverter.Convert(TEST, null, 8, null);

        Assert.That(output, Is.EqualTo("Test str.."));
    }

    [Test]
    public void PropertiesSetter_CanSetProperty()
    {
        const string TEST_VALUE = "Test value";

        PropertiesSetter.SetParameters(_parametrizedClass, ("String", TEST_VALUE));

        Assert.That(_parametrizedClass.String, Is.EqualTo(TEST_VALUE));
    }

    [Test]
    public void PropertiesSetter_ThrowsIsDidNotFoundRequiredProperty()
    {
        Assert.Throws<InvalidOperationException>(() => PropertiesSetter.SetParameters(_parametrizedClass, ("_number", 15)));
    }
    
    [Test]
    public void PropertiesSetter_ThrowsIsDidNotFoundPropertyWithPublicSet()
    {
        Assert.Throws<InvalidOperationException>(() => PropertiesSetter.SetParameters(_parametrizedClass, ("PrivateNumber", "test")));
    }

    [Test]
    public void PropertiesSetter_SetsParameterOnce_IfTwoSameParameters ()
    {
        const string TEST_1 = "test 1";
        const string TEST_2 = "test 2";

        PropertiesSetter.SetParameters(_parametrizedClass, ("String", TEST_1), ("String", TEST_2));

        Assert.That(_parametrizedClass.String, Is.EqualTo(TEST_1));
    }
}