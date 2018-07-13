using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Futurez.Xrm.XrmToolbox.Controls
{
    [Serializable]
    public enum EnumLoggingLevel
    {
        None,
        Exception,
        Information,
        Verbose
    }
    [Serializable]
    public enum EnumEntityTypes
    {
        Custom,
        System,
        BothCustomAndSystem
    }
    [Serializable]
    public enum ListViewColumnDisplayMode
    {
        Expanded,
        Compact
    }
    [Serializable]
    public enum EnumFilterMatchType
    {
        Equals,
        Contains,
        StartsWith,
        EndsWith,
        RegEx
    }

    [DisplayName("Filter Criteria")]
    [Category("Filter Settings")]
    [Description("Class containing information about exclusion filters")]
    [DefaultProperty("FilterString")]
    [Serializable]
    public class FilterInfo
    {
        [DisplayName("Filter Text")]
        [Description("Provide the filter text to be applied")]
        [Category("Filter Settings")]
        public string FilterString{ get; set; }

        [DisplayName("Filter Match")]
        [Description("Choose how this Filter String will be applied")]
        [Category("Filter Settings")]
        public EnumFilterMatchType FilterMatchType { get; set; }

        public override string ToString() {
            return FilterMatchType.ToString() + ": " + FilterString;
        }
    }

    [Serializable]
    public class ConfigurationInfo
    {
        #region Constructor 
        public ConfigurationInfo() {

            EntityFilters = new List<FilterInfo>() {
                new FilterInfo() { FilterString = "syncerror", FilterMatchType = EnumFilterMatchType.Equals }
            };

            EntityTypes = EnumEntityTypes.BothCustomAndSystem;
            RetrieveAsIfPublished = true;
        }
        #endregion

        #region Helper methods 

        /// <summary>
        /// Helper method that will tell whether to filter a given Entity based on its Logical Name
        /// </summary>
        /// <param name="logicalName">Entity Logical Name</param>
        /// <returns></returns>
        public bool FilterEntity(string logicalName)
        {
            return FilterItem(logicalName, EntityFilters.ToList<FilterInfo>());
        }

        /// <summary>
        /// Iterate through all of the filters for the specific item, return true if it matches one of the filter criteria
        /// </summary>
        /// <param name="matchName">Item to be matched, such as Attribute Schema Name or Entity Logical Name</param>
        /// <param name="filters">Filter Info collection</param>
        private bool FilterItem(string matchName, List<FilterInfo> filters)
        {
            if (filters == null) {
                return false;
            }

            matchName = matchName.ToLower();

            // default to false.  must find a match to return true... allows for empty list
            bool filtersMatch = false;
            foreach (var filter in filters)
            {
                switch (filter.FilterMatchType)
                {
                    case EnumFilterMatchType.Contains:
                        filtersMatch = matchName.Contains(filter.FilterString.ToLower());
                        break;
                    case EnumFilterMatchType.EndsWith:
                        filtersMatch = matchName.EndsWith(filter.FilterString.ToLower());
                        break;
                    case EnumFilterMatchType.StartsWith:
                        filtersMatch = matchName.StartsWith(filter.FilterString.ToLower());
                        break;
                    case EnumFilterMatchType.Equals:
                        filtersMatch = (matchName == filter.FilterString.ToLower());
                        break;
                    case EnumFilterMatchType.RegEx:
                        Regex regex = new Regex(filter.FilterString);
                        Match match = regex.Match(matchName);
                        filtersMatch = match.Success;
                        break;
                }
                if (filtersMatch == true) {
                    break;
                }
            }

            return filtersMatch;
        }

        #endregion

        #region Public properties
        [DisplayName("Entity Types")]
        [Description("Which Entity types should be loaded on retrieve.")]
        [Category("Filter Settings")]
        public EnumEntityTypes EntityTypes { get; set; }

        [DisplayName("Entity Filters")]
        [Description("List of filters to be applied to Entity retrieval and generation. These are Entities that you want to be excluded from the list and not generated in the template.")]
        [Category("Filter Settings")]
        [ListBindable(BindableSupport.Yes)]
        public List<FilterInfo> EntityFilters { get; set; }

        [DisplayName("Retrieve As If Published")]
        [Description("Flag indicating whether to retrieve the metadata that has not been published")]
        [Category("Project Settings")]
        public bool RetrieveAsIfPublished { get; set; }

        [DisplayName("Column Display Mode")]
        [Description("Display additional column details or Name and Entity Logical Name only")]
        [Category("Project Settings")]
        public ListViewColumnDisplayMode ColumnDisplayMode { get; set; }
        
        [DisplayName("Logging Level")]
        [Description("Toggle to enable logging while generating the templates")]
        [Category("Project Settings")]
        public EnumLoggingLevel LoggingLevel { get; set; }

        #endregion
    }
}
