﻿using System;
using System.IO;
using System.Threading.Tasks;
using CLI.Signals;
using Microsoft.Extensions.CommandLineUtils;

namespace CLI.Commands
{
    public partial class CommandsBuilder
    {

        public static void CreateWatchCommand(CommandLineApplication app)
        {
            _ = app.Command("watch", (command) =>
                  {
                      command.Description = "Watch a .car Project";
                      command.HelpOption("-?|-h|--help");

                      var fileOption = command.Option(
                            "-d|--dir <directory>",
                            "The directory of your zdragon project.",
                            CommandOptionType.SingleValue);

                      var serve = command.Option(
                            "-s|--serve",
                            "Serve files from the out folder in a simple static file server.",
                            CommandOptionType.NoValue);

                      command.OnExecute(() =>
                      {
                          var directory = fileOption.HasValue() switch
                          {
                              false => Directory.GetCurrentDirectory(),
                              true => fileOption.Value()
                          };

                          var project = new Project(directory);

                          Task webserverTask = null;
                          if (serve.HasValue())
                          {
                              webserverTask = WebServer.Start(project.OutPath);
                          }

                          project.Watch();

                          // Wait for the user to quit the program.
                          Console.WriteLine("Press 'q' to quit the sample.");
                          while (Console.ReadKey().Key != ConsoleKey.Q) { }


                          SignalSingleton.ExitSignal.Dispatch();
                          project.Dispose();
                          return 0;
                      });
                  });
        }
    }
}
