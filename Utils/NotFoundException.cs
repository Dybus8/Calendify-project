using System;
using System.Security.Cryptography;
using System.Text;


namespace StarterKit.Utils;
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}