using Desktop_Client.Core.Tools.Attributes;

namespace TestProject;

[Parameter(typeof(string), "String")]
internal class ClassWithParameters
{
    public string String { get; set; }

    public string PrivateString { get; private set; }
}