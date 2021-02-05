using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System.IO;
using System.Linq;
using static FileHopper.Util;
using FileHopper.Properties;
using Grasshopper.Kernel.Parameters;

namespace FileHopper
{
    public class GetFile : GH_Component
    {

        public GetFile()
          : base("GetFile", "File",
              "Append a file to a directory path through utilising wildcard matching or auto-filled value lists ",
              "Params", "FileHopper")
        {
            this.Params.ParameterSourcesChanged += UpdateValueList;
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "P", "Windows directory to search within", GH_ParamAccess.item);
            pManager.AddTextParameter("File[]", "F[]", "Pattern to match files against", GH_ParamAccess.item);
        }
        private readonly int pathParamIndex = 0;
        private readonly int fileParamIndex = 1;

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Path","P", "Path with appended file(s)", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            string path = "";
            DA.GetData(pathParamIndex, ref path);

            string file = "";
            DA.GetData(fileParamIndex, ref file);

            var pathsOut = new string[0];

            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                var files = dir.GetFiles();
                var goodFiles = files.Where(f => f.Name.WildCardMatch(file));
                pathsOut = goodFiles.Select(goodFile => goodFile.FullName).ToArray();
            }
            DA.SetDataList(0, pathsOut);

        }

        void UpdateValueList(object sender, GH_ParamServerEventArgs e)
        {
            if (e.ParameterIndex == fileParamIndex)
            {
                var valueLists = GetConnectedValueLists();
                if (valueLists.Count > 1)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Multiple value lists connected, only the first value list that was connected will be populated with files");
                }
                if (valueLists.Count >= 1)
                {
                    var valueList = valueLists[0];
                    var paths = GetPaths();
                    HashSet<string> newFileNames = GetFiles(paths);
                    HashSet<string> oldFileNames = GetValueListNames(valueList);
                    if (!oldFileNames.SetEquals(newFileNames))
                    {
                        var newNameValuePairs = newFileNames.Select(name => (name, name.ToValueListString()));
                        RebuildValueList(valueList, newNameValuePairs);
                    }

                }
            }
        }

        private List<GH_ValueList> GetConnectedValueLists()
        {
            return this.Params.Input[fileParamIndex].Sources.Where(source => source is GH_ValueList)
                                                              .Select(source => (GH_ValueList)source)
                                                              .ToList();
        }

        private List<DirectoryInfo> GetPaths()
        {
            return this.Params.Input[pathParamIndex].VolatileData.AllData(true)
                                                                 .Select(data => ((GH_String)data).Value)
                                                                 .Select(path => new DirectoryInfo(path))
                                                                 .ToList();
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.File;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("b2879ae1-e745-4c61-908b-7bf914e76aeb"); }
        }
    }
}