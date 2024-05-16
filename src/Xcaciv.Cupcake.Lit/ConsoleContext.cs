using System.Diagnostics;
using System.Xml.Linq;
using Xcaciv.Command;
using Xcaciv.Command.Interface;

namespace Xcaciv.Cupcake.Lit;

/// <summary>
/// console messge context that operates syncornesly
/// </summary>
internal class ConsoleContext : AbstractTextIo
{
    /// <summary>
    /// template for progress text output
    /// 0 - context name
    /// 1 - int progress
    /// </summary>
    public string ProgressTemplate { get; set; } = "{0} progress {1}%";

    public ConsoleContext(string name, Guid? parentId = null) : base(name, parentId)
    {
        
    }

    /// <summary>
    /// get a new
    /// </summary>
    /// <returns></returns>
    public override Task<ITextIoContext> GetChild(string[]? childArguments = null)
    {
        return Task.FromResult(new ConsoleContext(Name, Id) { ProgressTemplate = this.ProgressTemplate } as ITextIoContext);
    }

    public override Task HandleOutputChunk(string chunk)
    {
        Console.WriteLine(chunk);
        return Task.CompletedTask;
    }

    /// <summary>
    /// get input from user
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public override Task<string> PromptForCommand(string prompt)
    {
        Console.Write(prompt);
        return Task.FromResult(Console.ReadLine() ?? String.Empty);
    }

    /// <summary>
    /// do some progress math, print a progress message and return the int progress
    /// </summary>
    /// <param name="total"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public override Task<int> SetProgress(int total, int step)
    {
        int progress = total / step;
        this.OutputChunk(String.Format(this.ProgressTemplate, this.Name, progress));
        return Task.FromResult(progress);
    }

    /// <summary>
    /// output status message to console
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public override Task SetStatusMessage(string message)
    {
        Debug.WriteLine(message);
        return Task.CompletedTask;
    }

}
