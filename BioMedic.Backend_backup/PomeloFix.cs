using System.Runtime.CompilerServices;

namespace BioMedic.Backend
{
    public static class PomeloFix
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
    }
}