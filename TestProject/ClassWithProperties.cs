using Desktop_Client.Core.Tools.Attributes;

namespace TestProject;

[Parameter(typeof(string), "String")]
[Parameter(typeof(string), "PrivateString")]
internal class ClassWithParameters
{
    public string String { get; set; }

    public string PrivateString { get; private set; }
}