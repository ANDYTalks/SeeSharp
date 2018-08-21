using SeeSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace SeeSharp
{
    enum InvalidArgument
    {
        ADAPTER,
        FILTER,
        MISC,
        NUM_ARGS
    }

    class AppMain
    {
        private Seer seer;

        public static HashSet<string> AcceptableCommandEntries { get => acceptableCommandEntries; set => acceptableCommandEntries = value; }

        static void Main(string[] args)
        {
            AppMain application = new AppMain();

            application.HandleArguments(args);
            Console.Read();
        }

        public void HandleArguments(string[] args)
        {
            seer = new Seer();
            LinkedList<Tuple<int, InvalidArgument>> invalidArguments = new LinkedList<Tuple<int, InvalidArgument>>();
            bool allValid = InterpretEnteredCommandArguments(args, invalidArguments);
            if (!allValid) HandleInvalidCommandArguments(args, invalidArguments);
            seer.CaptureLivePackets();
        }

        private void HandleInvalidCommandArguments(string[] args, LinkedList<Tuple<int, InvalidArgument>> invalidArguments)
        {
            if (invalidArguments.Count > 0)
            {
                foreach (var arg in invalidArguments)
                {
                    InvalidArgument enumeral = arg.Item2;
                    switch (enumeral)
                    {
                        case InvalidArgument.ADAPTER:
                            Console.WriteLine($"Invalid adapter argument entered: {args[arg.Item1]}");
                            seer.DeviceForUse = seer.SelectPacketDevice();
                            break;
                        case InvalidArgument.FILTER:
                            Console.WriteLine($"Invalid filter argument entered: {args[arg.Item1]}");
                            if (seer.DeviceForUse == null)
                                seer.DeviceForUse = seer.SelectPacketDevice();
                            break;
                        case InvalidArgument.MISC:
                            Console.WriteLine($"Invalid command-line argument entered: {args[arg.Item1]}");
                            if (seer.DeviceForUse == null)
                                seer.DeviceForUse = seer.SelectPacketDevice();
                            break;
                    }
                }
            }
        }

        private bool InterpretEnteredCommandArguments(string[] args, LinkedList<Tuple<int, InvalidArgument>> invalidArguments)
        {
            bool allValid = false;
            if (args.Length > 0)
            {
                for (int ndx = 0; ndx < args.Length; ++ndx)
                {
                    var ArgWithoutHyphens = args[ndx].ToLowerInvariant().Trim().Replace("-", String.Empty);

                    if (AcceptableCommandEntries.Any(ArgWithoutHyphens.Equals))
                    {
                        switch (ArgWithoutHyphens)
                        {
                            case "a":
                            case "ad":
                            case "adapter":
                                //Console.WriteLine($"ndx+1: {ndx + 1} args.Length: {args.Length} ndx+1 < args.Length: {(ndx + 1) < args.Length}");
                                if ((ndx + 1) < args.Length)
                                {
                                    string inIndex = args[ndx + 1].Trim();
                                    bool wasInteger = Int32.TryParse(args[++ndx], out int adapterIndex);

                                    if (wasInteger)
                                    {
                                        seer.DeviceForUse = seer.SelectPacketDevice(adapterIndex);
                                        Console.WriteLine($"Capture will begin on {seer.ProvideCaptureDeviceDescription(seer.DeviceForUse)}");
                                        allValid = true;
                                    }
                                    else
                                    {
                                        invalidArguments.AddLast(Tuple.Create(ndx - 1, InvalidArgument.ADAPTER));
                                    }
                                }
                                else
                                {
                                    invalidArguments.AddLast(Tuple.Create(ndx, InvalidArgument.ADAPTER));
                                }
                                break;
                            case "f":
                            case "fi":
                            case "filter":
                                // Console.WriteLine($"ndx+1: {ndx + 1} args.Length: {args.Length} ndx+1 < args.Length: {(ndx + 1) < args.Length}");
                                if ((ndx + 1) < args.Length)
                                {
                                    if (Seer.DetermineFilterInputHasValidCharacters(args[++ndx]))
                                    {
                                        seer.CaptureFilter = args[ndx];
                                    }

                                    else
                                    {
                                        invalidArguments.AddLast(Tuple.Create(ndx, InvalidArgument.FILTER));
                                    }
                                }
                                else
                                {
                                    invalidArguments.AddLast(Tuple.Create(ndx, InvalidArgument.FILTER));
                                }
                                break;
                        }
                    }
                    else
                    {
                        invalidArguments.AddLast(Tuple.Create(ndx++, InvalidArgument.MISC));
                        Console.WriteLine("Whoops");
                    }

                    //Console.WriteLine(ndx);
                }

            }

            else if (args.Length == 0)
            {

                seer.DeviceForUse = seer.SelectPacketDevice();
                Console.WriteLine(seer.ProvideCaptureDeviceDescription(seer.DeviceForUse));
                allValid = true;
            }

            return allValid;
        }

        private static HashSet<String> acceptableCommandEntries = new HashSet<string>()
            {
                "a", "ad", "adapter",
                "f","fi", "filter"
            };
    }


}
