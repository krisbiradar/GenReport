using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenReport.Infrastructure.Models.HttpResponse.Shared
{
    /// <summary>
    /// Represents a sidebar item response.
    /// </summary>
    public class SidebarImplResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the sidebar item.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the sidebar item.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the CSS class for the icon associated with the sidebar item.
        /// </summary>
        public required string IconClass { get; set; }

        /// <summary>
        /// Gets or sets the description of the sidebar item.
        /// </summary>
        public required string Description { get; set; } = string.Empty;
    }
}