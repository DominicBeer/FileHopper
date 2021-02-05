using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.IO;
using System.Linq;
using static FileHopper.Util;
using FileHopper.Properties;



namespace FileHopper
{
    public class GetFolder : GH_Component
    {

        public GetFolder()
          : base("GetFolder", "Fldr",
              "Extends a path with all folders that match wildcard pattern",
              "Params", "FileHopper")
        {
            this.Params.ParameterSourcesChanged += UpdateValueList;
        }
        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_FilePath(),"Path", "P", "Windows directory to search within", GH_ParamAccess.item);
            pManager.AddTextParameter("Folder[]", "F[]", "Pattern to match folders against", GH_ParamAccess.item);
        }
        private readonly int pathParamIndex = 0;
        private readonly int folderParamIndex = 1;

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "P", "Path with appended folder(s)", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
            string path = "";
            DA.GetData(pathParamIndex, ref path);

            string folder = "";
            DA.GetData(folderParamIndex, ref folder);

            var pathsOut = new string[0];

            var dir = new DirectoryInfo(path);
            if(dir.Exists)
            {
                var subDirs = dir.GetDirectories();
                var goodDirs = subDirs.Where(subDir => subDir.Name.WildCardMatch(folder));
                pathsOut = goodDirs.Select(goodDir => goodDir.FullName).ToArray();
            }
            DA.SetDataList(0, pathsOut);
            
        }
                
        void UpdateValueList(object sender, GH_ParamServerEventArgs e)
        {
            if (e.ParameterIndex == folderParamIndex)
            {
                var valueLists = GetConnectedValueLists();
                if (valueLists.Count > 1)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Multiple value lists connected, only first value list will be populated with files");
                }
                
                if (valueLists.Count >= 1)
                {
                    var valueList = valueLists[0];
                    
                    var paths = GetPaths();
                    HashSet<string> newFolderNames = GetFolders(paths);
                    HashSet<string> oldFolderNames = GetValueListNames(valueList);
                    if (!oldFolderNames.SetEquals(newFolderNames))
                    {
                        var newNameValuePairs = newFolderNames.Select(name => (name, name.ToValueListString()));
                        RebuildValueList(valueList, newNameValuePairs);
                    }
                }
            }
        }

        private List<GH_ValueList> GetConnectedValueLists()
        {
            return this.Params.Input[folderParamIndex].Sources.Where(source => source is GH_ValueList)
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
                return Resources.Folder;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("590a587b-a8ed-486b-ac5a-0b4630075978"); }
        }
    }
}
