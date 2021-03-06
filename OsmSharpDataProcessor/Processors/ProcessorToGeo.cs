﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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
using OsmSharpDataProcessor.Processors.GTFS;
using OsmSharpDataProcessor.Processors.RouterDbs;
using System;
using System.Collections.Generic;

namespace OsmSharpDataProcessor.Processors
{
    /// <summary>
    /// Represents a processor can be used to convert streams to geometries.
    /// </summary>
    public class ProcessorToGeo : ProcessorBase
    {
        private Dictionary<string, string> _parameters;

        /// <summary>
        /// Creates a new to geo processor.
        /// </summary>
        public ProcessorToGeo()
        {
            _parameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Creates a new to geo processor.
        /// </summary>
        public ProcessorToGeo(Dictionary<string, string> parameters)
        {
            _parameters = new Dictionary<string, string>(parameters);
        }

        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
        /// </summary>
        public override int Collapse(List<ProcessorBase> processors, int i)
        {
            if (processors == null) { throw new ArgumentNullException("processors"); }
            if (processors.Count == 0) { throw new ArgumentOutOfRangeException("processors", "There has to be at least on processor there to collapse this filter."); }
            if (i < 1) { throw new ArgumentOutOfRangeException("i"); }
            
            if (processors[i - 1] is IGTFSSource)
            { // take all processors that are sources for this merge operation.
                var source = processors[i - 1] as IGTFSSource;
                processors[i] = new GTFS.ProcessorConvertToGeo(true, true);
                processors[i].Meta = processors[i - 1].Meta;
            }
            else if (processors[i - 1] is IRouterDbSource)
            { // take all processors that are sources for this merge operation.
                var source = processors[i - 1] as IRouterDbSource;
                processors[i] = new RouterDbs.Shape.RouterDbProcessorToGeo();
                processors[i].Meta = processors[i - 1].Meta;
            }
            return -1;
        }

        /// <summary>
        /// Returns true if this processor is ready.
        /// </summary>
        public override bool IsReady
        { // a source is always ready.
            get { return true; }
        }

        /// <summary>
        /// Executes the tasks or commands in this processor.
        /// </summary>
        public override void Execute()
        { // a source cannot be executed.
            throw new InvalidOperationException("This processor cannot be executed, check CanExecute before calling this method.");
        }

        /// <summary>
        /// Returns true if this processor can be executed.
        /// </summary>
        public override bool CanExecute
        { // a source cannot be executed.
            get { return false; }
        }
    }
}