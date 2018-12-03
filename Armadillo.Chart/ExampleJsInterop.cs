using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Armadillo.Chart
{
    public class ExampleJsInterop
    {
        // public static Task RenderNetwork()
        // {
        //     return JSRuntime.Current.InvokeAsync<string>("RenderNetwork");
        // } 

        public static Task<string> Prompt(string message)
        {
            // Implemented in exampleJsInterop.js
            return JSRuntime.Current.InvokeAsync<string>(
                "exampleJsFunctions.showPrompt",
                message);
        }
    }
}
