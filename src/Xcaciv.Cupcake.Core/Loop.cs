using Xcaciv.Command.Interface;
using Xcaciv.Command;

namespace Xcaciv.Cupcake.Core;

public class Loop
{
    public string Prompt { get; set; } = "Ɛ> ";
    public List<string> ExitCommands { get; set; } = new List<string>() { "END", "EXIT", "BYEE" };
    public string PackageDirectory { get; set; } = @".\packages";
    public async Task RunAsync(ITextIoContext context)
    {
        await context.SetStatusMessage("Loading Commands").ConfigureAwait(false);
        var controller = new CommandController();

        try
        {
            controller.AddPackageDirectory(this.PackageDirectory);
            controller.LoadCommands();
        }
        catch (Exception ex)
        {
            throw new Exceptions.LoadingException("Unable to load commands.", ex);
        }

        await context.SetStatusMessage("Done").ConfigureAwait(false);

        await Task.Run(async () => {
            var inputCommand = "";
            while (!this.ExitCommands.Contains(inputCommand, StringComparer.OrdinalIgnoreCase))
            {
                // run inputCommand if it was not blank
                if (String.IsNullOrEmpty(inputCommand)) await controller.Run(inputCommand, context);
                // get next inputCommand
                inputCommand = await context.PromptForCommand(this.Prompt).ConfigureAwait(false);

                // TODO: figure out how to handle non existing controller: download, compile
                // TODO: support NuGet style directory structure
            }
        });
        
    }
    /// <summary>
    /// run the inputCommand loop synchronously using inputFunc to get the commandline
    /// </summary>
    /// <param name="context"></param>
    public void Run(ITextIoContext context)
    {
        context.SetStatusMessage("Loading Commands").Wait();
        var controller = new CommandController();

        try
        {
            controller.AddPackageDirectory(this.PackageDirectory);
            controller.LoadDefaultCommands();
            controller.LoadCommands();
        }
        catch (Xcaciv.Command.Exceptions.NoPluginsFoundException)
        {
            context.SetStatusMessage("No Plugins Found. You may want to check out `install --help`").Wait();
            // TODO: download first plugin and GOTO start again! :D
            // throw new Exceptions.LoadingException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new Exceptions.LoadingException("Unable to load commands.", ex);
        }

        var inputCommand = "";
        while (!this.ExitCommands.Contains(inputCommand, StringComparer.OrdinalIgnoreCase))
        {
            // run inputCommand if it was not blank
            if (!String.IsNullOrEmpty(inputCommand)) controller.Run(inputCommand, context).Wait();
            // get next inputCommand
            inputCommand = context.PromptForCommand(this.Prompt).Result;
        }    
        
    }
}
