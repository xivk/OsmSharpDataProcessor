﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Streams.Complete;
using OsmSharp.Osm.Streams.Filters;
using OsmSharpDataProcessor.CommandLine;
using OsmSharpDataProcessor.Streams;
using System;

namespace OsmSharpDataProcessor
{
    internal class Program
    {
        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            OsmSharp.Logging.Log.Enable();
            OsmSharp.Logging.Log.RegisterListener(new OsmSharp.WinForms.UI.Logging.ConsoleTraceListener());

            // parse commands first.
            Command[] commands = CommandParser.ParseCommands(args);

            // convert commands into data processors.
            if (commands == null || commands.Length < 2)
            {
                throw new Exception("Please specifiy a valid data processing command!");
            }

            // start from the final command, that should be a target.
            object processor = commands[commands.Length - 1].CreateProcessor();
            if (!(processor is OsmStreamTarget) && !(processor is OsmCompleteStreamTarget))
            {
                throw new InvalidCommandException(
                    string.Format("Last argument {0} does not present a data processing target!",
                                  commands[commands.Length - 1].ToString()));
            }

            // target is defined.
            object target = processor;

            // get the second to last argument.
            processor = commands[commands.Length - 2].CreateProcessor();
            if (!(processor is OsmStreamSource))
            {
                throw new InvalidCommandException(
                    string.Format("Second last argument {0} does not present a data processing source or filter!",
                                  commands[commands.Length - 2].ToString()));
            }

            // three options: merge/filter/source.
            if (processor is MergedOsmStreamSource)
            { // special case: register all source with this merge-filter.
                var mergeStream = (processor as MergedOsmStreamSource);
                if (target is OsmStreamTarget)
                {
                    (target as OsmStreamTarget).RegisterSource(mergeStream);
                }
                else if (target is OsmCompleteStreamTarget)
                {
                    (target as OsmCompleteStreamTarget).RegisterSource(mergeStream);
                }
                int commandIdx = commands.Length - 3;
                while (commandIdx >= 0)
                {
                    processor = commands[commandIdx].CreateProcessor();

                    if (processor is OsmStreamFilter)
                    {
                        throw new InvalidCommandException("No filter allowed before a merge.");
                    }
                    else if(processor is OsmStreamTarget)
                    {
                        throw new InvalidCommandException("No targets allowed before a merge.");
                    }
                    else if (processor is OsmStreamSource)
                    { // register this source for the merge operation.
                        var source = (processor as OsmStreamSource);
                        mergeStream.RegisterSource(source);
                    }

                    // move to next command.
                    commandIdx--;
                }
            }
            else if (processor is OsmStreamFilter)
            {
                // there should be more filters or sources.
                var filter = (processor as OsmStreamFilter);
                if (target is OsmStreamTarget)
                {
                    (target as OsmStreamTarget).RegisterSource(filter);
                }
                else if (target is OsmCompleteStreamTarget)
                {
                    (target as OsmCompleteStreamTarget).RegisterSource(filter);
                }

                int commandIdx = commands.Length - 3;
                while (commandIdx >= 0)
                {
                    processor = commands[commandIdx].CreateProcessor();

                    // check source/filter.
                    if (!(processor is OsmStreamSource))
                    {
                        throw new InvalidCommandException(
                            string.Format(
                                "Second last argument {0} does not present a data processing source or filter!",
                                commands[commands.Length - 2].ToString()));
                    }

                    if (processor is OsmStreamFilter)
                    {
                        // another filter!
                        var newFilter = (processor as OsmStreamFilter);
                        filter.RegisterSource(newFilter);
                        filter = newFilter;
                    }
                    else if (processor is OsmStreamSource)
                    {
                        // everything should end here!
                        var source = (processor as OsmStreamSource);
                        source = new OsmStreamFilterProgress(source);
                        filter.RegisterSource(source);

                        if (commandIdx > 0)
                        {
                            throw new InvalidCommandException(
                                string.Format("Wrong order in filter/source specification!"));
                        }
                    }

                    // move to next command.
                    commandIdx--;
                }
            }
            else if (processor is OsmStreamSource)
            {
                // everything should end here!
                var source = (processor as OsmStreamSource);
                source = new OsmStreamFilterProgress(source);
                if (target is OsmStreamTarget)
                {
                    (target as OsmStreamTarget).RegisterSource(source);
                }
                else if (target is OsmCompleteStreamTarget)
                {
                    (target as OsmCompleteStreamTarget).RegisterSource(
                        source);
                }

                if (commands.Length > 2)
                {
                    throw new InvalidCommandException(
                        string.Format("Wrong order in filter/source specification!"));
                }
            }

            // execute the command by pulling the data to the target.                
            if (target is OsmStreamTarget)
            {
                (target as OsmStreamTarget).Pull();
            }
            else if (target is OsmCompleteStreamTarget)
            {
                (target as OsmCompleteStreamTarget).Pull();
            }
        }
    }
}