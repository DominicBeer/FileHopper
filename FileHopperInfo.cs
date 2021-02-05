using System;
using System.Drawing;
using FileHopper.Properties;
using Grasshopper.Kernel;

namespace FileHopper
{
    public class FileHopperInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "FileHopper";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return Resources.Logo;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "Provides functionality for navigating the Windows file system in Grasshopper through wildcard pattern matching and auto filled value lists";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("eb75741b-5fda-4066-81d0-2086220009d7");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Dominic Beer";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "tbc";
            }
        }
    }
}
